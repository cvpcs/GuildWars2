using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using LibHotKeys;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ObservableCollectionLogger logger = new ObservableCollectionLogger();
        private readonly AppConfigManager configManager;
        private readonly OcrManager ocrManager;

        private ScoreBar redScoreBar = new ScoreBar("Red score bar", "./resources/redbar_background.png", "./resources/redbar_boost.png", "./resources/redbar_score.png", true);
        private ScoreBar blueScoreBar = new ScoreBar("Blue score bar", "./resources/bluebar_background.png", "./resources/bluebar_boost.png", "./resources/bluebar_score.png");

        // TODO: can this be removed and just reference the logger directly?
        public ObservableCollection<string> ConsoleOutput => this.logger.Collection;

        private bool isLiveSetupEnabled = true;
        public bool IsLiveSetupEnabled
        {
            get => this.isLiveSetupEnabled;
            set
            {
                // TODO: consider moving this logic elsewhere
                if (this.isLiveSetupEnabled != value)
                {
                    this.isLiveSetupEnabled = value;

                    if (this.isLiveSetupEnabled) this.ocrManager.ProcessedScreenshot += OcrManager_ProcessedScreenshot;
                    else this.ocrManager.ProcessedScreenshot -= OcrManager_ProcessedScreenshot;

                    this.OcrProcessedScreenshotViewImage.Effect = this.isLiveSetupEnabled ? null : new BlurEffect();

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLiveSetupEnabled)));
                }
            }
        }

        public bool IsOverlayMode
        {
            get => this.redScoreBar.IsOverlayMode && this.blueScoreBar.IsOverlayMode;
            set
            {
                if (IsOverlayMode != value)
                {
                    this.redScoreBar.IsOverlayMode =
                    this.blueScoreBar.IsOverlayMode = value;

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverlayMode)));
                }
            }
        }

        // TODO: this constructor is overloaded with too many event handler definitions
        //       need to move this somewhere else
        public MainWindow()
        {
            InitializeComponent();
            
            this.logger.Collection.CollectionChanged += (s, e) => Dispatcher.Invoke(() => ConsoleScroller.ScrollToBottom());

            this.configManager = new AppConfigManager(this.logger);

            this.ocrManager = new OcrManager
            {
                RedSection = new Rectangle((int)(0.42708 * SystemParameters.PrimaryScreenWidth),
                                           (int)(0.00000 * SystemParameters.PrimaryScreenHeight),
                                           (int)(0.04167 * SystemParameters.PrimaryScreenWidth),
                                           (int)(0.03704 * SystemParameters.PrimaryScreenHeight)),
                BlueSection = new Rectangle((int)(0.53125 * SystemParameters.PrimaryScreenWidth),
                                            (int)(0.00000 * SystemParameters.PrimaryScreenHeight),
                                            (int)(0.04167 * SystemParameters.PrimaryScreenWidth),
                                            (int)(0.03704 * SystemParameters.PrimaryScreenHeight))
            };


            this.ocrManager.ScoresRead += (scores) => Dispatcher.Invoke(() =>
            {
                if (scores.IsValid)
                {
                    this.SetScoreBarFillPercentage(scores.RedPercentage, scores.BluePercentage);
                }

                this.logger.LogInformation($"Red: {scores.Red}, Blue: {scores.Blue}");
            });

            this.ocrManager.ProcessingFailed += (e) => Dispatcher.Invoke(() => this.logger.LogError(e, string.Empty));

            this.redScoreBar.Window.Show();
            this.blueScoreBar.Window.Show();

            this.RedSectionConfig.PropertyChanged += (s, e) =>
            {
                this.ocrManager.RedSection = this.RedSectionConfig.SectionRect;
                this.redScoreBar.Window.SetScoreBarModulation(this.RedSectionConfig.ScoreBarModulationParameters);
            };
            this.BlueSectionConfig.PropertyChanged += (s, e) =>
            {
                this.ocrManager.BlueSection = this.BlueSectionConfig.SectionRect;
                this.blueScoreBar.Window.SetScoreBarModulation(this.BlueSectionConfig.ScoreBarModulationParameters);
            };

            this.RedSectionConfig.SectionRect = this.ocrManager.RedSection;
            this.BlueSectionConfig.SectionRect = this.ocrManager.BlueSection;

            this.ocrManager.CapturedScreenshot += (size) => Dispatcher.Invoke(() =>
            {
                this.RedSectionConfig.MaxWidth = size.Width;
                this.RedSectionConfig.MaxHeight = size.Height;
                this.BlueSectionConfig.MaxWidth = size.Width;
                this.BlueSectionConfig.MaxHeight = size.Height;
            });

            this.ocrManager.ProcessedScreenshot += OcrManager_ProcessedScreenshot;

            this.ocrManager.StartThread();

            HotKeyManager.HotKeyPressed += (_) => Dispatcher.Invoke(() =>
            {
                try
                {
                    using (Bitmap bmp = new Gw2Process().GetBitmap())
                    {
                        // TODO: this was added to allow for multiple screenshots for OBS to update to reduce lag.
                        //       consider a better way to support this
                        const int numScreenshotCopies = 4;
                        for (var i = 0; i < numScreenshotCopies; i++)
                        {
                            var screenshotFile = $"screenshot{i:00}.png";
                            bmp.Save($"./{screenshotFile}", ImageFormat.Png);
                            this.logger.LogInformation($"Saved screenshots to {screenshotFile}");
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, string.Empty);
                }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            this.ocrManager.StopThread();

            this.redScoreBar.Window.Close();
            this.blueScoreBar.Window.Close();

            base.OnClosed(e);
        }

        public void FillScores_Clicked(object sender, EventArgs args)
            => Dispatcher.Invoke(() => this.SetScoreBarFillPercentage(1, 1));

        public void ClearScores_Clicked(object sender, EventArgs args)
            => Dispatcher.Invoke(() => this.SetScoreBarFillPercentage(0, 0));

        public void SaveConfig_Clicked(object sender, EventArgs args)
        {
            this.configManager.TrySave(new AppConfig
            {
                UseLiveSetup = IsLiveSetupEnabled,
                UseOverlayMode = IsOverlayMode,
                RedSection = RedSectionConfig.SectionRect,
                BlueSection = BlueSectionConfig.SectionRect,
                RedScoreBarPosition = this.redScoreBar.Window.GetWindowRect(),
                BlueScoreBarPosition = this.blueScoreBar.Window.GetWindowRect(),
                RedScoreBarModulation = RedSectionConfig.ScoreBarModulationParameters,
                BlueScoreBarModulation = BlueSectionConfig.ScoreBarModulationParameters,
                ScreenshotHotKey = ScreenshotHotKey
            });
        }

        public void LoadConfig_Clicked(object sender, EventArgs args)
        {
            if (this.configManager.TryLoad(out AppConfig config))
            {
                IsLiveSetupEnabled = config.UseLiveSetup;
                IsOverlayMode = config.UseOverlayMode;
                RedSectionConfig.SectionRect = config.RedSection;
                BlueSectionConfig.SectionRect = config.BlueSection;
                this.redScoreBar.Window.SetWindowRect(config.RedScoreBarPosition);
                this.blueScoreBar.Window.SetWindowRect(config.BlueScoreBarPosition);
                RedSectionConfig.ScoreBarModulationParameters = config.RedScoreBarModulation;
                BlueSectionConfig.ScoreBarModulationParameters = config.BlueScoreBarModulation;
                ScreenshotHotKey = config.ScreenshotHotKey;
            }
        }

        private (int id, HotKey hotKey)? screenshotHotKeyRegistration = null;
        private HotKey ScreenshotHotKey
        {
            get => this.screenshotHotKeyRegistration?.hotKey;
            set
            {
                if (this.screenshotHotKeyRegistration?.hotKey != value)
                {
                    if (this.screenshotHotKeyRegistration.HasValue) HotKeyManager.UnregisterHotKey(this.screenshotHotKeyRegistration.Value.id);
                    this.screenshotHotKeyRegistration = (HotKeyManager.RegisterHotKey(value), value);
                }
            }
        }

        public void SetScreenshotHotKey_Clicked(object sender, EventArgs args)
        {
            var messageBox = new Window
            {
                Content = new TextBlock
                {
                    Text = "Press any key combination to bind it to the screenshot hotkey.\n\n"
                         + $"The current hotkey is: {screenshotHotKeyRegistration?.hotKey.ToString()}",
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                },
                Owner = this,
                Width = 300,
                Height = 100,
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None
            };

            messageBox.KeyDown += (s, e) =>
            {
                e.Handled = true;
                switch (e.Key == Key.System ? e.SystemKey : e.Key)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                    case Key.LeftShift:
                    case Key.RightShift:
                    case Key.LeftAlt:
                    case Key.RightAlt:
                    case Key.LWin:
                    case Key.RWin:
                        return;

                    case Key.Escape:
                        messageBox.Close();
                        break;

                    default:
                        ScreenshotHotKey = new HotKey(e.Key, e.KeyboardDevice.Modifiers);
                        messageBox.Close();
                        break;
                }
            };

            messageBox.Show();
        }

        private void OcrManager_ProcessedScreenshot(OcrManager.ProcessedScreenshotEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                WriteableBitmap bitmapDisplay = this.OcrProcessedScreenshotViewImage.Source as WriteableBitmap;
                if (bitmapDisplay == null ||
                    bitmapDisplay.Width != args.Screenshot.Width ||
                    bitmapDisplay.Height != args.Screenshot.Height)
                {
                    bitmapDisplay = new WriteableBitmap(args.Screenshot.Width, args.Screenshot.Height,
                                                        args.Screenshot.HorizontalResolution, args.Screenshot.VerticalResolution,
                                                        PixelFormats.Bgra32, null);
                }

                bitmapDisplay.WriteBitmap(args.Screenshot);
                bitmapDisplay.WriteBitmap(args.RedSectionPreProcessedScreenshot, args.RedSection);
                bitmapDisplay.WriteBitmap(args.BlueSectionPreProcessedScreenshot, args.BlueSection);

                this.OcrProcessedScreenshotViewImage.Source = bitmapDisplay;
            });
        }

        private void SetScoreBarFillPercentage(double red, double blue)
        {
            this.redScoreBar.Window.SetScoreBarFill(red);
            this.blueScoreBar.Window.SetScoreBarFill(blue);
        }
    }
}
