using System;
using System.ServiceModel;

namespace GuildWars2.Overlay.Contract
{
    public static class OverlayManager
    {
        private static IOverlay m_OVERLAY = null;
        private static IOverlay OVERLAY
        {
            get
            {
                if (m_OVERLAY == null)
                {
                    ChannelFactory<IOverlay> overlayChannel = new ChannelFactory<IOverlay>(
                            new NetNamedPipeBinding(),
                            new EndpointAddress(string.Format("net.pipe://localhost/{0}", typeof(IOverlay).FullName)));
                    m_OVERLAY = overlayChannel.CreateChannel();
                }

                return m_OVERLAY;
            }
        }

        public static void Reload()
        {
            try
            {
                OVERLAY.Reload();
            }
            catch (Exception) { }
        }

        public static void LoadHTML(string html)
        {
            try
            {
                OVERLAY.LoadHTML(html);
            }
            catch (Exception) { }
        }

        public static void LoadUri(Uri uri)
        {
            try
            {
                OVERLAY.LoadUri(uri);
            }
            catch (Exception) { }
        }

        public static void ExecuteJavascript(string js)
        {
            try
            {
                OVERLAY.ExecuteJavascript(js);
            }
            catch (Exception) { }
        }
    }
}
