using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GuildWars2.PvPCasterToolbox.Configuration;

namespace GuildWars2.PvPCasterToolbox.TabPages
{
    public partial class GameTracking : TabItem
    {
        public GameTracking(Gw2ScreenshotProcessor gw2ScreenshotProcessor,
                            AppConfig appConfig)
        { 
            InitializeComponent();

            this.RedScoreCropConfig.CropRect = appConfig.RedSection;
            this.BlueScoreCropConfig.CropRect = appConfig.BlueSection;

            gw2ScreenshotProcessor.ScreenshotCaptured += screenshot => Dispatcher.Invoke(() =>
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

                this.OcrProcessedScreenshotViewImage.Source = bitmapDisplay;

                this.RedScoreCropConfig.ImageWidth = this.BlueScoreCropConfig.ImageWidth = (uint)bitmapDisplay.PixelWidth;
                this.RedScoreCropConfig.ImageHeight = this.BlueScoreCropConfig.ImageHeight = (uint)bitmapDisplay.PixelHeight;
            });
            
            this.RedScoreCropConfig.PropertyChanged += (s, e) => appConfig.RedSection = this.RedScoreCropConfig.CropRect;
            this.BlueScoreCropConfig.PropertyChanged += (s, e) => appConfig.BlueSection = this.BlueScoreCropConfig.CropRect;

            appConfig.PropertyChanged += (s, e) => Dispatcher.Invoke(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(appConfig.RedSection):
                        this.RedScoreCropConfig.CropRect = appConfig.RedSection;
                        break;

                    case nameof(appConfig.BlueSection):
                        this.BlueScoreCropConfig.CropRect = appConfig.BlueSection;
                        break;
                }
            });
        }
    }
}
