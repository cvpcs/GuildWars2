using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Awesomium.Core;

using GuildWars2.Overlay.Contract;

namespace GuildWars2.Overlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class MainWindow : Window, IOverlay
    {
        private ServiceHost selfHost;
        private WindowInteropHelper interop;

        public MainWindow()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Awesomium.Source = new Uri(args[1]);
            }

            Awesomium.IsTransparent = true;

            Awesomium.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.LeftShift)
                    {
                        WindowUI.Visibility = (WindowUI.IsVisible ? Visibility.Hidden : Visibility.Visible);
                    }

                    if (e.Key == Key.LeftCtrl)
                    {
                        this.IsClickThroughTransparent = !this.IsClickThroughTransparent;
                    }
                };

            WindowUI.Visibility = Visibility.Visible;

            this.selfHost = new ServiceHost(this, new Uri("net.pipe://localhost"));
            this.selfHost.AddServiceEndpoint(typeof(IOverlay), new NetNamedPipeBinding(), typeof(IOverlay).FullName);
            this.selfHost.Open();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.interop = new WindowInteropHelper(this);

            // set window to layered
            int initialStyle = GetWindowLong(this.interop.Handle, GWL_EXSTYLE);
            SetWindowLong(this.interop.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.selfHost.Close();

            // shutdown awesomium
            Awesomium.Dispose();
            WebCore.Shutdown();
        }

        #region Click-through transparency
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);

        private bool IsClickThroughTransparent
        {
            get
            {
                int style = GetWindowLong(this.interop.Handle, GWL_EXSTYLE);
                return (style & WS_EX_TRANSPARENT) == WS_EX_TRANSPARENT;
            }
            set
            {
                int style = GetWindowLong(this.interop.Handle, GWL_EXSTYLE);

                if (value)
                {
                    style |= WS_EX_TRANSPARENT;
                }
                else
                {
                    style &= ~WS_EX_TRANSPARENT;
                }

                SetWindowLong(this.interop.Handle, GWL_EXSTYLE, style);
            }
        }
        #endregion

        #region IOverlay Contract

        public void Reload()
        {
            Awesomium.Reload(true);
        }

        public void LoadUri(Uri uri)
        {
            Awesomium.Source = uri;
        }

        public void LoadHTML(string html)
        {
            Awesomium.LoadHTML(html);
        }

        public void ExecuteJavascript(string js)
        {
            Awesomium.ExecuteJavascript(js);
        }

        #endregion
    }
}
