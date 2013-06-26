using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private MapLayer m_PointsOfInterest;
        private MapLayer m_Vistas;
        private MapLayer m_RenownHearts;
        private MapLayer m_SkillPoints;
        private IDictionary<int, IList<UIElement>> m_MapElements;

        public MainWindow()
        {
            InitializeComponent();

            m_Link = new MumbleLink();
            m_MapData = new Dictionary<int, FloorMapDetails>();

            m_Player = new Pushpin();
            m_Player.Template = (ControlTemplate)Application.Current.Resources["PlayerPushpin"];
            m_Player.PositionOrigin = PositionOrigin.Center;
            m_Player.Visibility = Visibility.Hidden;
            m_Map.Children.Add(m_Player);

            m_Waypoints = new MapLayer();
            m_PointsOfInterest = new MapLayer();
            m_Vistas = new MapLayer();
            m_RenownHearts = new MapLayer();
            m_SkillPoints = new MapLayer();

            m_Map.Children.Add(m_Waypoints);
            m_Map.Children.Add(m_PointsOfInterest);
            m_Map.Children.Add(m_Vistas);
            m_Map.Children.Add(m_RenownHearts);
            m_Map.Children.Add(m_SkillPoints);

            m_MapElements = new Dictionary<int, IList<UIElement>>();

            ControlTemplate waypointPushpin = (ControlTemplate)Application.Current.Resources["WaypointPushpin"];
            ControlTemplate pointOfInterestPushpin = (ControlTemplate)Application.Current.Resources["PointOfInterestPushpin"];
            ControlTemplate vistaPushpin = (ControlTemplate)Application.Current.Resources["VistaPushpin"];
            ControlTemplate renownHeartPushpin = (ControlTemplate)Application.Current.Resources["RenownHeartPushpin"];
            ControlTemplate skillPointPushpin = (ControlTemplate)Application.Current.Resources["SkillPointPushpin"];

            MapFloorResponse response = new MapFloorRequest(1, 1).Execute();
            if (response != null)
            {
                foreach (FloorRegion region in response.Regions.Values)
                {
                    foreach (string mapId in region.Maps.Keys)
                    {
                        int mid = int.Parse(mapId);
                        FloorMapDetails map = region.Maps[mapId];

                        m_MapData.Add(mid, map);
                        m_MapElements.Add(mid, new List<UIElement>());

                        foreach (PointOfInterest poi in map.PointsOfInterest)
                        {
                            Pushpin poiPin = new Pushpin();
                            poiPin.PositionOrigin = PositionOrigin.Center;
                            poiPin.Location = m_Map.Unproject(new Point(poi.Coord[0], poi.Coord[1]), m_Map.MaxZoomLevel);
                            poiPin.Visibility = Visibility.Hidden;

                            if (!string.IsNullOrWhiteSpace(poi.Name))
                                poiPin.ToolTip = poi.Name;

                            switch (poi.TypeEnum)
                            {
                                case PointOfInterestType.Waypoint:
                                    poiPin.Template = waypointPushpin;
                                    m_Waypoints.Children.Add(poiPin);
                                    break;
                                case PointOfInterestType.Landmark:
                                    poiPin.Template = pointOfInterestPushpin;
                                    m_PointsOfInterest.Children.Add(poiPin);
                                    break;
                                case PointOfInterestType.Vista:
                                    poiPin.Template = vistaPushpin;
                                    m_Vistas.Children.Add(poiPin);
                                    break;
                                default:
                                    continue;
                            }

                            m_MapElements[mid].Add(poiPin);
                        }

                        foreach (Task rh in map.Tasks)
                        {
                            Pushpin rhPin = new Pushpin();
                            rhPin.PositionOrigin = PositionOrigin.Center;
                            rhPin.Location = m_Map.Unproject(new Point(rh.Coord[0], rh.Coord[1]), m_Map.MaxZoomLevel);
                            rhPin.Visibility = Visibility.Hidden;
                            rhPin.Template = renownHeartPushpin;

                            if (!string.IsNullOrWhiteSpace(rh.Objective))
                                rhPin.ToolTip = string.Format("{0} ({1})", rh.Objective, rh.Level);

                            m_RenownHearts.Children.Add(rhPin);
                            m_MapElements[mid].Add(rhPin);
                        }

                        foreach (MappedModel sp in map.SkillChallenges)
                        {
                            Pushpin spPin = new Pushpin();
                            spPin.PositionOrigin = PositionOrigin.Center;
                            spPin.Location = m_Map.Unproject(new Point(sp.Coord[0], sp.Coord[1]), m_Map.MaxZoomLevel);
                            spPin.Visibility = Visibility.Hidden;
                            spPin.Template = skillPointPushpin;
                            m_SkillPoints.Children.Add(spPin);
                            m_MapElements[mid].Add(spPin);
                        }
                    }
                }
            }

            m_Map.ViewChangeEnd += (s, e) =>
                {
                    foreach (IList<UIElement> elements in m_MapElements.Values)
                    {
                        foreach (UIElement element in elements)
                            element.Visibility = Visibility.Hidden;
                    }

                    if (m_Map.ZoomLevel >= 3)
                    {
                        int mid = GetMapByCenter(m_Map.Project(m_Map.Center, m_Map.MaxZoomLevel));
                        if (m_MapElements.ContainsKey(mid))
                        {
                            foreach (UIElement element in m_MapElements[mid])
                                element.Visibility = Visibility.Visible;
                        }
                    }
                };

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

        private int GetMapByCenter(Point center)
        {
            foreach (KeyValuePair<int, FloorMapDetails> entry in m_MapData)
            {
                if (center.X > entry.Value.ContinentRect[0][0] &&
                        center.X < entry.Value.ContinentRect[1][0] &&
                        center.Y > entry.Value.ContinentRect[0][1] &&
                        center.Y < entry.Value.ContinentRect[1][1])
                    return entry.Key;
            }

            return -1;
        }

        private double TranslateX(double x, List<List<double>> mapRect, List<List<double>> continentRect)
        {
            return (x - mapRect[0][0]) / (mapRect[1][0] - mapRect[0][0]) * (continentRect[1][0] - continentRect[0][0]) + continentRect[0][0];
        }

        private double TranslateZ(double z, List<List<double>> mapRect, List<List<double>> continentRect)
        {
            return (-z - mapRect[0][1]) / (mapRect[1][1] - mapRect[0][1]) * (continentRect[1][1] - continentRect[0][1]) + continentRect[0][1];
        }

        private void WorkerThread()
        {
            while (m_WorkerRunning)
            {
                try
                {
                    if (m_Link.DataAvailable && m_MapData.ContainsKey(m_Link.Map))
                    {
                        FloorMapDetails map = m_MapData[m_Link.Map];

                        // convert back to inches
                        double posX = m_Link.PositionX * 39.3700787;
                        double posZ = m_Link.PositionZ * 39.3700787;
                        double rot = m_Link.RotationPlayer;

                        posX = TranslateX(posX, map.MapRect, map.ContinentRect);
                        posZ = TranslateZ(posZ, map.MapRect, map.ContinentRect);

                        // move the player icon
                        Dispatcher.Invoke(() =>
                            {
                                m_Player.Location = m_Map.Unproject(new Point(posX, posZ), m_Map.MaxZoomLevel);
                                m_Player.Heading = rot;
                                m_Player.Visibility = Visibility.Visible;
                            }, DispatcherPriority.Render, new CancellationToken(), new TimeSpan(0, 0, 1));
                    }
                }
                catch
                { }

                Thread.Sleep(100);
            }
        }
    }
}
