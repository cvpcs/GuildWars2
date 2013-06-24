using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Awesomium.Core;
using Awesomium.Windows.Controls;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MumbleLink m_Link;
        private IDictionary<int, FloorMapDetails> m_MapData;
        private int m_TimerSync;
        private System.Timers.Timer m_Timer;

        public MainWindow()
        {
            InitializeComponent();

            m_Link = new MumbleLink();
            m_MapData = new Dictionary<int, FloorMapDetails>();

            foreach(FloorRegion region in new MapFloorRequest(1, 1).Execute().Regions.Values)
            {
                foreach (string mapId in region.Maps.Keys)
                {
                    m_MapData.Add(int.Parse(mapId), region.Maps[mapId]);
                }
            }

            m_TimerSync = 0;
            m_Timer = new System.Timers.Timer(100);
            m_Timer.Elapsed += TimerElapsed;

            // load the html once the native view is ready
            Awesomium.NativeViewInitialized += (o, e) =>
                {
                    Awesomium.LoadHTML(Properties.Resources.map_html);
                    m_Timer.Start();
                };
        }

        protected override void OnClosed(EventArgs e)
        {
            m_Timer.Stop();

            // wait for any existing threads to complete
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref m_TimerSync, -1, 0) == 0);

            base.OnClosed(e);

            // shutdown awesomium
            Awesomium.Dispose();
            WebCore.Shutdown();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // attempt to set the sync, if another of us is running, just exit
            if (Interlocked.CompareExchange(ref m_TimerSync, 1, 0) != 0)
                return;

            try
            {
                FloorMapDetails map = m_MapData[m_Link.Map];

                // convert back to inches
                double posX = m_Link.PositionX * 39.3700787;
                double posZ = m_Link.PositionZ * 39.3700787;

                posX = (double)(posX - map.MapRect[0][0]) / (double)(map.MapRect[1][0] - map.MapRect[0][0]) * (map.ContinentRect[1][0] - map.ContinentRect[0][0]) + map.ContinentRect[0][0];
                posZ = (double)(-posZ - map.MapRect[0][1]) / (double)(map.MapRect[1][1] - map.MapRect[0][1]) * (map.ContinentRect[1][1] - map.ContinentRect[0][1]) + map.ContinentRect[0][1];

                Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            Awesomium.ExecuteJavascript(string.Format("updatePlayer('{0}', {1}, {2}, {3});", m_Link.PlayerName, posX, posZ, m_Link.RotationPlayer));
                        }
                        catch
                        { }
                    });
            }
            catch
            { }

            // reset sync to 0
            Interlocked.Exchange(ref m_TimerSync, 0);
        }
    }
}
