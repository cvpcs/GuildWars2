﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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

        private ScoreBarWindow redScoreBarWindow = new RedScoreBarWindow();
        private ScoreBarWindow blueScoreBarWindow = new BlueScoreBarWindow();

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

        private bool isOverlayMode = true;
        public bool IsOverlayMode
        {
            get => this.isOverlayMode;
            set
            {
                // TODO: can this logic be moved into the scorebar window definition? maybe a static method?
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
                    this.redScoreBarWindow?.SetScoreBarFill(scores.RedPercentage);
                    this.blueScoreBarWindow?.SetScoreBarFill(scores.BluePercentage);
                }

                this.logger.LogInformation($"Red: {scores.Red}, Blue: {scores.Blue}");
            });

            this.ocrManager.ProcessingFailed += (e) => Dispatcher.Invoke(() => this.logger.LogError(e, string.Empty));

            this.redScoreBarWindow.Show();
            this.blueScoreBarWindow.Show();

            this.RedSectionConfig.Title = "Red score section position";
            this.BlueSectionConfig.Title = "Blue score section position";

            this.RedSectionConfig.PropertyChanged += (s, e) =>
            {
                this.ocrManager.RedSection = this.RedSectionConfig.SectionRect;
                this.redScoreBarWindow.SetScoreBarModulation(this.RedSectionConfig.ScoreBarModulationParameters);
            };
            this.BlueSectionConfig.PropertyChanged += (s, e) =>
            {
                this.ocrManager.BlueSection = this.BlueSectionConfig.SectionRect;
                this.blueScoreBarWindow.SetScoreBarModulation(this.BlueSectionConfig.ScoreBarModulationParameters);
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
                        bmp.Save("./screenshot.png", ImageFormat.Png);
                    }

                    this.logger.LogInformation("Saved screenshot to screenshot.png");
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

            this.redScoreBarWindow.Close();
            this.blueScoreBarWindow.Close();

            base.OnClosed(e);
        }

        public void FillScores_Clicked(object sender, EventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                this.redScoreBarWindow.SetScoreBarFill(1);
                this.blueScoreBarWindow.SetScoreBarFill(1);
            });
        }

        public void ClearScores_Clicked(object sender, EventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                this.redScoreBarWindow.SetScoreBarFill(0);
                this.blueScoreBarWindow.SetScoreBarFill(0);
            });
        }

        public void SaveConfig_Clicked(object sender, EventArgs args)
        {
            this.configManager.TrySave(new AppConfig
            {
                UseLiveSetup = IsLiveSetupEnabled,
                UseOverlayMode = IsOverlayMode,
                RedSection = RedSectionConfig.SectionRect,
                BlueSection = BlueSectionConfig.SectionRect,
                RedScoreBarPosition = this.redScoreBarWindow.GetWindowRect(),
                BlueScoreBarPosition = this.blueScoreBarWindow.GetWindowRect(),
                RedScoreBarModulation = RedSectionConfig.ScoreBarModulationParameters,
                BlueScoreBarModulation = BlueSectionConfig.ScoreBarModulationParameters
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
                this.redScoreBarWindow.SetWindowRect(config.RedScoreBarPosition);
                this.blueScoreBarWindow.SetWindowRect(config.BlueScoreBarPosition);
                RedSectionConfig.ScoreBarModulationParameters = config.RedScoreBarModulation;
                BlueSectionConfig.ScoreBarModulationParameters = config.BlueScoreBarModulation;
            }
        }

        private int? registeredHotkey;
        public void SetScreenshotHotkey_Clicked(object sender, EventArgs args)
        {
            var messageBox = new Window
            {
                Content = new TextBlock
                {
                    Text = "Press any key combination to bind it to the screenshot key"
                },
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None
            };

            bool ctrl = false;
            bool shift = false;
            bool alt = false;
            bool win = false;

            messageBox.KeyDown += (s, e) =>
            {
                e.Handled = true;
                switch (e.Key == Key.System ? e.SystemKey : e.Key)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        ctrl = true;
                        break;

                    case Key.LeftShift:
                    case Key.RightShift:
                        shift = true;
                        break;

                    case Key.LeftAlt:
                    case Key.RightAlt:
                        alt = true;
                        break;

                    case Key.LWin:
                    case Key.RWin:
                        win = true;
                        break;

                    default:
                        KeyModifiers modifiers = KeyModifiers.NoRepeat;
                        if (ctrl) modifiers |= KeyModifiers.Control;
                        if (shift) modifiers |= KeyModifiers.Shift;
                        if (alt) modifiers |= KeyModifiers.Alt;
                        if (win) modifiers |= KeyModifiers.Windows;
                        if (registeredHotkey.HasValue) HotKeyManager.UnregisterHotKey(registeredHotkey.Value);
                        registeredHotkey = HotKeyManager.RegisterHotKey((Keys)KeyInterop.VirtualKeyFromKey(e.Key), modifiers);
                        messageBox.Close();
                        break;
                }
            };
            messageBox.KeyUp += (s, e) =>
            {
                e.Handled = true;
                switch (e.Key == Key.System ? e.SystemKey : e.Key)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        ctrl = false;
                        break;

                    case Key.LeftShift:
                    case Key.RightShift:
                        shift = false;
                        break;

                    case Key.LeftAlt:
                    case Key.RightAlt:
                        alt = false;
                        break;

                    case Key.LWin:
                    case Key.RWin:
                        win = false;
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
    }
}
