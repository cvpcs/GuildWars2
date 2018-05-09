using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GuildWars2.PvPCasterToolbox.Configuration;
using LibHotKeys;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPCasterToolbox.TabPages
{
    public partial class HotKeys : TabItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppConfig AppConfig { get; private set; }
        public string ScreenshotHotKey => AppConfig.ScreenshotHotKey?.ToString();

        private object screenshotHotKeyLock = new object();
        private int? screenshotHotKeyId = null;

        private int screenshotCaptureState = (int)CaptureState.Idle;

        private ILogger logger;

        public HotKeys(Gw2ScreenshotPublisher gw2ScreenshotPublisher,
                       AppConfig appConfig,
                       ILogger<HotKeys> logger)
        {
            AppConfig = appConfig;
            this.logger = logger;

            if (AppConfig.ScreenshotHotKey != null)
            {
                this.SetScreenshotHotKey(AppConfig.ScreenshotHotKey);
            }

            InitializeComponent();

            AppConfig.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppConfig.ScreenshotHotKey))
                {
                    this.SetScreenshotHotKey(AppConfig.ScreenshotHotKey);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScreenshotHotKey)));
                }
            };

            HotKeyManager.HotKeyPressed += hotKey =>
            {
                if (hotKey.Equals(AppConfig.ScreenshotHotKey))
                {
                    // set
                    Interlocked.CompareExchange(ref screenshotCaptureState, (int)CaptureState.Pending, (int)CaptureState.Idle);
                }
            };

            gw2ScreenshotPublisher.DataAvailable += screenshot =>
            {
                if (Interlocked.CompareExchange(ref screenshotCaptureState, (int)CaptureState.Active, (int)CaptureState.Pending) == (int)CaptureState.Pending)
                {
                    // TODO: this was added to allow for multiple screenshots for OBS to update to reduce lag.
                    //       consider a better way to support this
                    for (var i = 0; i < AppConfig.ScreenshotCount; i++)
                    {
                        var screenshotFile = $"screenshot{i:00}.png";
                        screenshot.Save($"./{screenshotFile}", ImageFormat.Png);
                        this.logger.LogInformation($"Saved screenshots to {screenshotFile}");
                    }

                    Interlocked.Exchange(ref screenshotCaptureState, (int)CaptureState.Idle);
                }
            };
        }

        // TODO: ICommand?
        public void ChangeScreenshotHotKey_Clicked(object sender, EventArgs args)
        {
            var messageBox = new Window
            {
                Content = new TextBlock
                {
                    Text = "Press any key combination to bind it to the screenshot hotkey.\n"
                         + "Press ESC to cancel or DEL to clear.",
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                },
                Owner = Window.GetWindow(this),
                Width = 250,
                Height = 80,
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

                    case Key.Delete:
                        this.SetScreenshotHotKey(null);
                        messageBox.Close();
                        break;

                    default:
                        this.SetScreenshotHotKey(new HotKey(e.Key, e.KeyboardDevice.Modifiers));
                        messageBox.Close();
                        break;
                }
            };

            messageBox.Show();
        }

        private void SetScreenshotHotKey(HotKey hotKey)
        {
            lock (screenshotHotKeyLock)
            {
                if (this.screenshotHotKeyId.HasValue)
                {
                    HotKeyManager.UnregisterHotKey(this.screenshotHotKeyId.Value);
                }

                this.screenshotHotKeyId = hotKey != null ? (int?)HotKeyManager.RegisterHotKey(hotKey) : null;
                AppConfig.ScreenshotHotKey = hotKey;
            }
        }

        private enum CaptureState
        {
            Idle = 0, // idle
            Pending = 1, // pending next screenshot
            Active = 2 // currently capturing
        }
    }
}
