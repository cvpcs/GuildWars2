using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string RedBarTitle = "Red score bar";
        private const string BlueBarTitle = "Blue score bar";

        private static readonly Uri RedBarBackgroundImageUri = new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/redbar_background.png"));
        private static readonly Uri RedBarBoostImageUri = new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/redbar_boost.png"));
        private static readonly Uri RedBarScoreImageUri = new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/redbar_score.png"));
        private static readonly Uri BlueBarBackgroundImageUri = new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/bluebar_background.png"));
        private static readonly Uri BlueBarBoostImageUri = new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/bluebar_boost.png"));
        private static readonly Uri BlueBarScoreImageUri = new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/bluebar_score.png"));

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly OcrManager ocrManager;

        private ScoreBarWindow redScoreBarWindow;
        private ScoreBarWindow blueScoreBarWindow;

        private ObservableCollection<string> consoleOutput = new ObservableCollection<string>();
        public ObservableCollection<string> ConsoleOutput
        {
            get => this.consoleOutput;
            set { this.consoleOutput = value; this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConsoleOutput))); }
        }

        private bool isLiveSetupEnabled = true;
        public bool IsLiveSetupEnabled
        {
            get => this.isLiveSetupEnabled;
            set
            {
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

        private bool isOverlayMode = true;
        public bool IsOverlayMode
        {
            get => this.isOverlayMode;
            set
            {
                if (this.isOverlayMode != value)
                {
                    this.isOverlayMode = value;

                    this.redScoreBarWindow.Close();
                    this.blueScoreBarWindow.Close();

                    var (newRedScoreBarWindow, newBlueScoreBarWindow) = CreateScoreBars(this.isOverlayMode);
                    newRedScoreBarWindow.Left = this.redScoreBarWindow.Left;
                    newRedScoreBarWindow.Top = this.redScoreBarWindow.Top;
                    newRedScoreBarWindow.Width = this.redScoreBarWindow.Width;
                    newRedScoreBarWindow.Height = this.redScoreBarWindow.Height;

                    newBlueScoreBarWindow.Left = this.blueScoreBarWindow.Left;
                    newBlueScoreBarWindow.Top = this.blueScoreBarWindow.Top;
                    newBlueScoreBarWindow.Width = this.blueScoreBarWindow.Width;
                    newBlueScoreBarWindow.Height = this.blueScoreBarWindow.Height;

                    this.redScoreBarWindow = newRedScoreBarWindow;
                    this.blueScoreBarWindow = newBlueScoreBarWindow;

                    this.redScoreBarWindow.Show();
                    this.blueScoreBarWindow.Show();

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverlayMode)));
                }
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();


            this.ocrManager = new OcrManager
            {
                RedSection = new Rectangle((int)(0.42708 * SystemParameters.PrimaryScreenWidth),
                                           (int)(0.00000 * SystemParameters.PrimaryScreenHeight),
                                           (int)(0.41667 * SystemParameters.PrimaryScreenWidth),
                                           (int)(0.03704 * SystemParameters.PrimaryScreenHeight)),
                BlueSection = new Rectangle((int)(0.53125 * SystemParameters.PrimaryScreenWidth),
                                            (int)(0.00000 * SystemParameters.PrimaryScreenHeight),
                                            (int)(0.41667 * SystemParameters.PrimaryScreenWidth),
                                            (int)(0.03704 * SystemParameters.PrimaryScreenHeight))
            };

            this.ocrManager.ScoresRead += (scores) => Dispatcher.Invoke(() =>
            {
                if (scores.IsValid)
                {
                    this.redScoreBarWindow?.SetScoreBarFill(scores.RedPercentage);
                    this.blueScoreBarWindow?.SetScoreBarFill(scores.BluePercentage);
                }

                ConsoleOutput.Add($"Red: {scores.Red}, Blue: {scores.Blue}");
                ConsoleScroller.ScrollToBottom();
            });

            (this.redScoreBarWindow, this.blueScoreBarWindow) = CreateScoreBars(true);
            this.redScoreBarWindow.Show();
            this.blueScoreBarWindow.Show();

            this.RedSectionConfig.Title = "Red score section position";
            this.RedSectionConfig.SectionRect = this.ocrManager.RedSection;
            this.BlueSectionConfig.Title = "Blue score section position";
            this.BlueSectionConfig.SectionRect = this.ocrManager.BlueSection;

            this.ocrManager.CapturedScreenshot += (size) => Dispatcher.Invoke(() =>
            {
                this.RedSectionConfig.MaxWidth = size.Width;
                this.RedSectionConfig.MaxHeight = size.Height;
                this.BlueSectionConfig.MaxWidth = size.Width;
                this.BlueSectionConfig.MaxHeight = size.Height;
            });

            this.RedSectionConfig.PropertyChanged += (s, e) => this.ocrManager.RedSection = this.RedSectionConfig.SectionRect;
            this.BlueSectionConfig.PropertyChanged += (s, e) => this.ocrManager.BlueSection = this.BlueSectionConfig.SectionRect;

            this.ocrManager.ProcessedScreenshot += OcrManager_ProcessedScreenshot;

            this.ocrManager.StartThread();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.ocrManager.StopThread();

            this.redScoreBarWindow?.Close();
            this.blueScoreBarWindow?.Close();

            base.OnClosed(e);
        }

        public void ClearScores_Clicked(object sender, EventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                this.redScoreBarWindow?.SetScoreBarFill(0);
                this.blueScoreBarWindow?.SetScoreBarFill(0);
            });
        }

        private static (ScoreBarWindow redScoreBarWindow, ScoreBarWindow blueScoreBarWindow) CreateScoreBars(bool overlayMode)
        {
            var redBarWindow = new ScoreBarWindow(RedBarBackgroundImageUri, RedBarBoostImageUri, RedBarScoreImageUri, true)
            {
                Title = RedBarTitle,
                AllowsTransparency = overlayMode
            };
            var blueBarWindow = new ScoreBarWindow(BlueBarBackgroundImageUri, BlueBarBoostImageUri, BlueBarScoreImageUri)
            {
                Title = BlueBarTitle,
                AllowsTransparency = overlayMode
            };

            redBarWindow.GreenScreen.Visibility =
            blueBarWindow.GreenScreen.Visibility = overlayMode ? Visibility.Hidden : Visibility.Visible;

            return (redBarWindow, blueBarWindow);
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

                BitmapData screenshotData = args.Screenshot.LockBits(new Rectangle(0, 0, args.Screenshot.Width, args.Screenshot.Height),
                                                                     ImageLockMode.ReadOnly, args.Screenshot.PixelFormat);
                BitmapData redSectionData = args.RedSectionPreProcessedScreenshot.LockBits(new Rectangle(0, 0, args.RedSectionPreProcessedScreenshot.Width, args.RedSectionPreProcessedScreenshot.Height),
                                                                                           ImageLockMode.ReadOnly, args.RedSectionPreProcessedScreenshot.PixelFormat);
                BitmapData blueSectionData = args.BlueSectionPreProcessedScreenshot.LockBits(new Rectangle(0, 0, args.BlueSectionPreProcessedScreenshot.Width, args.BlueSectionPreProcessedScreenshot.Height),
                                                                                             ImageLockMode.ReadOnly, args.BlueSectionPreProcessedScreenshot.PixelFormat);
                bitmapDisplay.WritePixels(new Int32Rect(0, 0, args.Screenshot.Width, args.Screenshot.Height),
                                          screenshotData.Scan0, screenshotData.Stride * screenshotData.Height, screenshotData.Stride);
                bitmapDisplay.WritePixels(new Int32Rect(args.RedSection.X, args.RedSection.Y, args.RedSection.Width, args.RedSection.Height),
                                          redSectionData.Scan0, redSectionData.Stride * redSectionData.Height, redSectionData.Stride);
                bitmapDisplay.WritePixels(new Int32Rect(args.BlueSection.X, args.BlueSection.Y, args.BlueSection.Width, args.BlueSection.Height),
                                          blueSectionData.Scan0, blueSectionData.Stride * blueSectionData.Height, blueSectionData.Stride);

                args.Screenshot.UnlockBits(screenshotData);
                args.RedSectionPreProcessedScreenshot.UnlockBits(redSectionData);
                args.BlueSectionPreProcessedScreenshot.UnlockBits(blueSectionData);

                this.OcrProcessedScreenshotViewImage.Source = bitmapDisplay;
            });
        }
    }
}
