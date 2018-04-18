using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using GuildWars2.PvPCasterToolbox.Configuration;
using GuildWars2.PvPCasterToolbox.GameState;

namespace GuildWars2.PvPCasterToolbox.TabPages
{
    public partial class GameTracking : TabItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GameStateManager gameStateManager;
        private AppConfig appConfig;

        private bool isLiveSetupEnabled = true;
        public bool IsLiveSetupEnabled
        {
            get => this.isLiveSetupEnabled;
            set
            {
                if (this.isLiveSetupEnabled != value)
                {
                    this.appConfig.UseLiveSetup =
                    this.isLiveSetupEnabled = value;

                    if (this.isLiveSetupEnabled) this.gameStateManager.ProcessedScreenshotSections += this.GameStateManager_ProcessedScreenshotSections;
                    else this.gameStateManager.ProcessedScreenshotSections -= this.GameStateManager_ProcessedScreenshotSections;

                    this.OcrProcessedScreenshotViewImage.Effect = this.isLiveSetupEnabled ? null : new BlurEffect();

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLiveSetupEnabled)));
                }
            }
        }

        public GameTracking(GameStateManager gameStateManager,
                            AppConfig appConfig)
        {
            this.gameStateManager = gameStateManager;
            this.appConfig = appConfig;

            InitializeComponent();

            this.RedScoreCropConfig.CropRect = this.appConfig.RedSection;
            this.BlueScoreCropConfig.CropRect = this.appConfig.BlueSection;

            if (appConfig.UseLiveSetup)
            {
                this.gameStateManager.ProcessedScreenshotSections += this.GameStateManager_ProcessedScreenshotSections;
            }
            
            this.RedScoreCropConfig.PropertyChanged += (s, e) => this.appConfig.RedSection = this.RedScoreCropConfig.CropRect;
            this.BlueScoreCropConfig.PropertyChanged += (s, e) => this.appConfig.BlueSection = this.BlueScoreCropConfig.CropRect;

            this.appConfig.PropertyChanged += (s, e) => Dispatcher.Invoke(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(this.appConfig.UseLiveSetup):
                        this.IsLiveSetupEnabled = this.appConfig.UseLiveSetup;
                        break;

                    case nameof(this.appConfig.RedSection):
                        this.RedScoreCropConfig.CropRect = this.appConfig.RedSection;
                        break;

                    case nameof(this.appConfig.BlueSection):
                        this.BlueScoreCropConfig.CropRect = this.appConfig.BlueSection;
                        break;
                }
            });
        }

        private void GameStateManager_ProcessedScreenshotSections(Bitmap screenshot, IEnumerable<(Rectangle rect, Bitmap bitmap)> sections)
        {
            Dispatcher.Invoke(() =>
            {
                WriteableBitmap bitmapDisplay = this.OcrProcessedScreenshotViewImage.Source as WriteableBitmap;
                if (bitmapDisplay == null ||
                    bitmapDisplay.Width != screenshot.Width ||
                    bitmapDisplay.Height != screenshot.Height)
                {
                    bitmapDisplay = new WriteableBitmap(screenshot.Width, screenshot.Height,
                                                        screenshot.HorizontalResolution, screenshot.VerticalResolution,
                                                        PixelFormats.Bgra32, null);
                }

                bitmapDisplay.WriteBitmap(screenshot);

                foreach (var (rect, bitmap) in sections)
                {
                    bitmapDisplay.WriteBitmap(bitmap, rect);
                }

                this.OcrProcessedScreenshotViewImage.Source = bitmapDisplay;

                this.RedScoreCropConfig.ImageWidth = this.BlueScoreCropConfig.ImageWidth = (uint)bitmapDisplay.PixelWidth;
                this.RedScoreCropConfig.ImageHeight = this.BlueScoreCropConfig.ImageHeight = (uint)bitmapDisplay.PixelHeight;
            });
        }
    }
}
