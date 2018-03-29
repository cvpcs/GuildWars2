using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;

using Awesomium.Core;

using GuildWars2.Overlay.Contract;
using GuildWars2.Overlay.Controls;

namespace GuildWars2.Overlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class MainWindow : ClickThroughTransparentWindow, IOverlay
    {
        private ServiceHost selfHost;

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.selfHost.Close();

            // shutdown awesomium
            Awesomium.Dispose();
            WebCore.Shutdown();
        }

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
