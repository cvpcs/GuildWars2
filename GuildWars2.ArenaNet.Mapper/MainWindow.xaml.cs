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
using System.Windows.Threading;

using Microsoft.Maps.MapControl.WPF;

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

        private bool m_WorkerRunning;
        private Thread m_WorkerThread;

        private Pushpin m_Player;
        private MapLayer m_Waypoints;

        public MainWindow()
        {
            InitializeComponent();

            m_Link = new MumbleLink();
            m_MapData = new Dictionary<int, FloorMapDetails>();

            m_Player = new Pushpin();
            m_Player.PositionOrigin = PositionOrigin.Center;
            m_Player.Template = (ControlTemplate)Application.Current.Resources["PlayerPushPin"];
            m_Map.Children.Add(m_Player);
            m_Waypoints = new MapLayer();
            m_Map.Children.Add(m_Waypoints);

            foreach(FloorRegion region in new MapFloorRequest(1, 1).Execute().Regions.Values)
            {
                foreach (string mapId in region.Maps.Keys)
                {
                    m_MapData.Add(int.Parse(mapId), region.Maps[mapId]);
                }
            }

            m_WorkerRunning = true;
            m_WorkerThread = new Thread(WorkerThread);
            m_WorkerThread.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            m_WorkerRunning = false;
            m_WorkerThread.Join();

            base.OnClosed(e);
        }

        private void WorkerThread()
        {
            while (m_WorkerRunning)
            {
                try
                {
                    FloorMapDetails map = m_MapData[m_Link.Map];

                    // convert back to inches
                    double posX = m_Link.PositionX * 39.3700787;
                    double posZ = m_Link.PositionZ * 39.3700787;
                    double rot = m_Link.RotationPlayer;

                    posX = (double)(posX - map.MapRect[0][0]) / (double)(map.MapRect[1][0] - map.MapRect[0][0]) * (map.ContinentRect[1][0] - map.ContinentRect[0][0]) + map.ContinentRect[0][0];
                    posZ = (double)(-posZ - map.MapRect[0][1]) / (double)(map.MapRect[1][1] - map.MapRect[0][1]) * (map.ContinentRect[1][1] - map.ContinentRect[0][1]) + map.ContinentRect[0][1];

                    // move the player icon
                    Dispatcher.Invoke(() =>
                        {
                            m_Player.Location = m_Map.Unproject(new Point(posX, posZ), m_Map.MaxZoomLevel);
                            m_Player.Heading = rot;
                        }, DispatcherPriority.Render, new CancellationToken(), new TimeSpan(0, 0, 1));
                }
                catch
                { }

                Thread.Sleep(100);
            }
        }
    }
}
