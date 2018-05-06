using System.ComponentModel;
using System.Windows.Controls;

namespace GuildWars2.PvPCasterToolbox.Controls
{
    /// <summary>
    /// Interaction logic for WindowUI.xaml
    /// </summary>
    public partial class ImageModulationConfig : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int scoreBarHue = 0;
        public int ScoreBarHue
        {
            get => this.scoreBarHue;
            set { if (this.scoreBarHue != value) { this.scoreBarHue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarHue))); } }
        }

        private int scoreBarSaturation = 0;
        public int ScoreBarSaturation
        {
            get => this.scoreBarSaturation;
            set { if (this.scoreBarSaturation != value) { this.scoreBarSaturation = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarSaturation))); } }
        }

        private int scoreBarBrightness = 0;
        public int ScoreBarBrightness
        {
            get => this.scoreBarBrightness;
            set { if (this.scoreBarBrightness != value) { this.scoreBarBrightness = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarBrightness))); } }
        }

        public ImageModulationParameters ScoreBarModulationParameters
        {
            get => new ImageModulationParameters(ScoreBarHue, ScoreBarSaturation, ScoreBarBrightness);
            set
            {
                ScoreBarHue = value?.Hue ?? default(int);
                ScoreBarSaturation = value?.Saturation ?? default(int);
                ScoreBarBrightness = value?.Brightness ?? default(int);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarModulationParameters)));
            }
        }

        public ImageModulationConfig()
        {
            InitializeComponent();
        }
    }
}
