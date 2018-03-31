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
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly OcrManager ocrManager;

        private ScoreBarWindow redScoreBarWindow = new RedScoreBarWindow();
        private ScoreBarWindow blueScoreBarWindow = new BlueScoreBarWindow();

        private ObservableCollectionLogger ConsoleLogger = new ObservableCollectionLogger();
        public ObservableCollection<string> ConsoleOutput => ConsoleLogger.Collection;

        private AppConfig config
        {
            get => new AppConfig
            {
                UseLiveSetup = IsLiveSetupEnabled,
                UseOverlayMode = IsOverlayMode,
                RedSection = RedSectionConfig.SectionRect,
                BlueSection = BlueSectionConfig.SectionRect,
                RedScoreBarPosition = this.redScoreBarWindow.GetWindowRect(),
                BlueScoreBarPosition = this.blueScoreBarWindow.GetWindowRect()
            };
            set
            {
                IsLiveSetupEnabled = value.UseLiveSetup;
                IsOverlayMode = value.UseOverlayMode;
                RedSectionConfig.SectionRect = value.RedSection;
                BlueSectionConfig.SectionRect = value.BlueSection;
                this.redScoreBarWindow.SetWindowRect(value.RedScoreBarPosition);
                this.blueScoreBarWindow.SetWindowRect(value.BlueScoreBarPosition);
            }
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

                    this.redScoreBarWindow = new RedScoreBarWindow(value, this.redScoreBarWindow.GetWindowRect());
                    this.blueScoreBarWindow = new BlueScoreBarWindow(value, this.blueScoreBarWindow.GetWindowRect());

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

                ConsoleLogger.LogInformation($"Red: {scores.Red}, Blue: {scores.Blue}");
                ConsoleScroller.ScrollToBottom();
            });

            this.ocrManager.ProcessingFailed += (e) => Dispatcher.Invoke(() => ConsoleLogger.LogError(e, string.Empty));

            this.redScoreBarWindow.Show();
            this.blueScoreBarWindow.Show();

            this.RedSectionConfig.Title = "Red score section position";
            this.BlueSectionConfig.Title = "Blue score section position";

            this.RedSectionConfig.PropertyChanged += (s, e) => this.config.RedSection = this.ocrManager.RedSection = this.RedSectionConfig.SectionRect;
            this.BlueSectionConfig.PropertyChanged += (s, e) => this.config.BlueSection = this.ocrManager.BlueSection = this.BlueSectionConfig.SectionRect;

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
        }

        protected override void OnClosed(EventArgs e)
        {
            this.ocrManager.StopThread();

            this.redScoreBarWindow.Close();
            this.blueScoreBarWindow.Close();

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

        public void SaveConfig_Clicked(object sender, EventArgs args)
        {
            var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json",
                Title = "Save configuration"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, JsonConvert.SerializeObject(this.config));
                }
                catch (Exception e)
                {
                    ConsoleLogger.LogError(e, string.Empty);
                }
            }
        }

        public void LoadConfig_Clicked(object sender, EventArgs args)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json",
                Multiselect = false,
                Title = "Load configuration"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    this.config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(dlg.FileName));
                }
                catch (Exception e)
                {
                    ConsoleLogger.LogError(e, string.Empty);
                }
            }
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
