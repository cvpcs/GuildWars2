using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Awesomium.Core;
using Awesomium.Windows.Controls;

using GuildWars2.Overlay.Contract;

namespace GuildWars2.Overlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class MainWindow : Window, IOverlay
    {
        ServiceHost m_SelfHost;

        public MainWindow()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                Awesomium.Source = new Uri(args[1]);

            Awesomium.IsTransparent = true;

            Awesomium.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.LeftShift)
                        WindowUI.Visibility = (WindowUI.IsVisible ? Visibility.Hidden : Visibility.Visible);
                };

            WindowUI.Visibility = Visibility.Visible;

            m_SelfHost = new ServiceHost(this, new Uri("net.pipe://localhost"));
            m_SelfHost.AddServiceEndpoint(typeof(IOverlay), new NetNamedPipeBinding(), typeof(IOverlay).FullName);
            m_SelfHost.Open();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            m_SelfHost.Close();

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
