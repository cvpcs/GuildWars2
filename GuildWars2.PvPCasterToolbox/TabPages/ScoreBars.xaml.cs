using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using GuildWars2.PvPCasterToolbox.Configuration;
using GuildWars2.PvPCasterToolbox.GameState;

namespace GuildWars2.PvPCasterToolbox.TabPages
{
    public partial class ScoreBars : TabItem, INotifyPropertyChanged
    {
        private ScoreBar redScoreBar = new ScoreBar("Red score bar", "./resources/redbar_background.png", "./resources/redbar_boost.png", "./resources/redbar_score.png", true);
        private ScoreBar blueScoreBar = new ScoreBar("Blue score bar", "./resources/bluebar_background.png", "./resources/bluebar_boost.png", "./resources/bluebar_score.png");

        private AppConfig appConfig;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsOverlayMode
        {
            get => this.redScoreBar.IsOverlayMode && this.blueScoreBar.IsOverlayMode;
            set
            {
                if (IsOverlayMode != value)
                {
                    this.appConfig.UseOverlayMode =
                    this.redScoreBar.IsOverlayMode =
                    this.blueScoreBar.IsOverlayMode = value;

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverlayMode)));
                }
            }
        }

        public ScoreBars(GameStateManager gameStateManager,
                         AppConfig appConfig)
        {
            this.appConfig = appConfig;

            this.redScoreBar.IsOverlayMode =
            this.blueScoreBar.IsOverlayMode = appConfig.UseOverlayMode;

            InitializeComponent();

            this.redScoreBar.Window.Show();
            this.blueScoreBar.Window.Show();

            this.Unloaded += (s, e) =>
            {
                this.redScoreBar.Window.Close();
                this.blueScoreBar.Window.Close();
            };

            this.RedScoreBarModulationConfig.PropertyChanged += (s, e) => Dispatcher.Invoke(() =>
            {
                this.redScoreBar.Window.SetScoreBarModulation(this.RedScoreBarModulationConfig.ScoreBarModulationParameters);
                this.appConfig.RedScoreBarModulation = this.RedScoreBarModulationConfig.ScoreBarModulationParameters;
            });

            this.BlueScoreBarModulationConfig.PropertyChanged += (s, e) => Dispatcher.Invoke(() =>
            {
                this.blueScoreBar.Window.SetScoreBarModulation(this.BlueScoreBarModulationConfig.ScoreBarModulationParameters);
                this.appConfig.BlueScoreBarModulation = this.BlueScoreBarModulationConfig.ScoreBarModulationParameters;
            });

            this.redScoreBar.PositionChanged += position => this.appConfig.RedScoreBarPosition = position;
            this.blueScoreBar.PositionChanged += position => this.appConfig.BlueScoreBarPosition = position;

            this.appConfig.PropertyChanged += (s, e) => Dispatcher.Invoke(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(this.appConfig.UseOverlayMode):
                        this.IsOverlayMode = this.appConfig.UseOverlayMode;
                        break;

                    case nameof(this.appConfig.RedScoreBarModulation):
                        this.RedScoreBarModulationConfig.ScoreBarModulationParameters = appConfig.RedScoreBarModulation;
                        break;

                    case nameof(this.appConfig.BlueScoreBarModulation):
                        this.BlueScoreBarModulationConfig.ScoreBarModulationParameters = appConfig.BlueScoreBarModulation;
                        break;

                    case nameof(this.appConfig.RedScoreBarPosition):
                        this.redScoreBar.Window.SetWindowRect(this.appConfig.RedScoreBarPosition);
                        break;

                    case nameof(this.appConfig.BlueScoreBarPosition):
                        this.blueScoreBar.Window.SetWindowRect(this.appConfig.BlueScoreBarPosition);
                        break;
                }
            });

            gameStateManager.ScoresRead += state =>
            {
                Dispatcher.Invoke(() => this.SetScoreBarFillPercentage(state.Red.ScorePercentage, state.Blue.ScorePercentage));

                // TODO: this should be done better
                File.WriteAllText("./redkills.txt", state.Red.Kills.ToString());
                File.WriteAllText("./bluekills.txt", state.Blue.Kills.ToString());

                // TODO: this is just a hack for testing, need to build this into a better system
                Action<string> playAudio = (file) => Task.Factory.StartNew(() =>
                {
                    if (File.Exists(file))
                    {
                        var mp = new MediaPlayer();
                        mp.Open(new Uri(Path.GetFullPath(file)));
                        mp.Play();
                    }
                });

                if (state.Red.KillsDelta > 0) playAudio("./redkills.mp3");
                if (state.Blue.KillsDelta > 0) playAudio("./bluekills.mp3");
            };
        }

        // TODO: ICommand?
        public void FillScores_Clicked(object sender, EventArgs args)
            => Dispatcher.Invoke(() => this.SetScoreBarFillPercentage(1, 1));

        // TODO: ICommand?
        public void ClearScores_Clicked(object sender, EventArgs args)
            => Dispatcher.Invoke(() => this.SetScoreBarFillPercentage(0, 0));

        private void SetScoreBarFillPercentage(double red, double blue)
        {
            this.redScoreBar.Window.SetScoreBarFill(red);
            this.blueScoreBar.Window.SetScoreBarFill(blue);
        }
    }
}
