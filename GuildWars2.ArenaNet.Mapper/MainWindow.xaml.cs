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

        private bool m_Running;
        private ManualResetEvent m_Canceled;
        private Thread m_PlayerWorkerThread;
        private Thread m_EventWorkerThread;

        private Pushpin m_Player;

        // map layers
        private IDictionary<int, MapLayer> m_MapLayers;
        private IDictionary<int, MapLayer> m_MapWaypoints;
        private IDictionary<int, MapLayer> m_MapPointsOfInterest;
        private IDictionary<int, MapLayer> m_MapVistas;
        private IDictionary<int, MapLayer> m_MapRenownHearts;
        private IDictionary<int, MapLayer> m_MapSkillPoints;
        private IDictionary<int, MapLayer> m_MapEvents;

        private IDictionary<Guid, EventMapLayer> m_EventElements;

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

            m_MapLayers = new Dictionary<int, MapLayer>();
            m_MapWaypoints = new Dictionary<int, MapLayer>();
            m_MapPointsOfInterest = new Dictionary<int, MapLayer>();
            m_MapVistas = new Dictionary<int, MapLayer>();
            m_MapRenownHearts = new Dictionary<int, MapLayer>();
            m_MapSkillPoints = new Dictionary<int, MapLayer>();
            m_MapEvents = new Dictionary<int, MapLayer>();

            m_EventElements = new Dictionary<Guid, EventMapLayer>();

            MapFloorResponse floor = new MapFloorRequest(1, 2).Execute();
            if (floor != null)
            {
                foreach (FloorRegion region in floor.Regions.Values)
                {
                    foreach (string mapId in region.Maps.Keys)
                    {
                        int mid = int.Parse(mapId);
                        FloorMapDetails map = region.Maps[mapId];

                        m_MapData.Add(mid, map);

                        m_MapLayers.Add(mid, new MapLayer());
                        m_MapLayers[mid].Visibility = Visibility.Hidden;
                        m_Map.Children.Add(m_MapLayers[mid]);

                        m_MapWaypoints.Add(mid, new MapLayer());
                        m_MapPointsOfInterest.Add(mid, new MapLayer());
                        m_MapVistas.Add(mid, new MapLayer());
                        m_MapRenownHearts.Add(mid, new MapLayer());
                        m_MapSkillPoints.Add(mid, new MapLayer());

                        m_MapLayers[mid].Children.Add(m_MapWaypoints[mid]);
                        m_MapLayers[mid].Children.Add(m_MapPointsOfInterest[mid]);
                        m_MapLayers[mid].Children.Add(m_MapVistas[mid]);
                        m_MapLayers[mid].Children.Add(m_MapRenownHearts[mid]);
                        m_MapLayers[mid].Children.Add(m_MapSkillPoints[mid]);

                        foreach (PointOfInterest poi in map.PointsOfInterest)
                        {
                            Pushpin poiPin = new PointOfInterestPushpin(poi);

                            poiPin.Location = m_Map.Unproject(new Point(poi.Coord[0], poi.Coord[1]), m_Map.MaxZoomLevel);

                            switch (poi.TypeEnum)
                            {
                                case PointOfInterestType.Waypoint:
                                    m_MapWaypoints[mid].Children.Add(poiPin);
                                    break;
                                case PointOfInterestType.Landmark:
                                    m_MapPointsOfInterest[mid].Children.Add(poiPin);
                                    break;
                                case PointOfInterestType.Vista:
                                    m_MapVistas[mid].Children.Add(poiPin);
                                    break;
                                default:
                                    continue;
                            }
                        }

                        foreach (Task rh in map.Tasks)
                        {
                            Pushpin rhPin = new TaskPushpin(rh);

                            rhPin.Location = m_Map.Unproject(new Point(rh.Coord[0], rh.Coord[1]), m_Map.MaxZoomLevel);

                            m_MapRenownHearts[mid].Children.Add(rhPin);
                        }

                        foreach (MappedModel sp in map.SkillChallenges)
                        {
                            Pushpin spPin = new SkillChallengePushpin(sp);

                            spPin.Location = m_Map.Unproject(new Point(sp.Coord[0], sp.Coord[1]), m_Map.MaxZoomLevel);

                            m_MapSkillPoints[mid].Children.Add(spPin);
                        }
                    }
                }
            }

            EventDetailsResponse events = new EventDetailsRequest().Execute();
            if (events != null)
            {
                foreach (KeyValuePair<string, EventDetails> entry in events.Events)
                {
                    Guid eid = new Guid(entry.Key);
                    EventDetails ev = entry.Value;

                    if (!ev.Name.StartsWith("skill challenge: ", StringComparison.InvariantCultureIgnoreCase) && m_MapData.ContainsKey(ev.MapId))
                    {
                        if (!m_MapEvents.ContainsKey(ev.MapId))
                        {
                            m_MapEvents.Add(ev.MapId, new MapLayer());

                            // we insert instead of add so events always show up under other pushpins
                            m_MapLayers[ev.MapId].Children.Insert(0, m_MapEvents[ev.MapId]);
                        }

                        FloorMapDetails map = m_MapData[ev.MapId];

                        EventMapLayer evLayer;

                        if (ev.Location.TypeEnum == LocationType.Poly)
                        {
                            EventPolyMapLayer evPolyLayer = new EventPolyMapLayer(ev);

                            foreach (List<double> pt in ev.Location.Points)
                            {
                                evPolyLayer.PolyLocations.Add(
                                        m_Map.Unproject(
                                                new Point(
                                                        TranslateX(pt[0], map.MapRect, map.ContinentRect),
                                                        TranslateZ(pt[1], map.MapRect, map.ContinentRect)),
                                                m_Map.MaxZoomLevel));
                            }

                            evLayer = evPolyLayer;
                        }
                        else
                        {
                            evLayer = new EventMapLayer(ev);
                        }

                        evLayer.Center = m_Map.Unproject(
                                new Point(
                                        TranslateX(ev.Location.Center[0], map.MapRect, map.ContinentRect),
                                        TranslateZ(ev.Location.Center[1], map.MapRect, map.ContinentRect)),
                                m_Map.MaxZoomLevel);

                        m_MapEvents[ev.MapId].Children.Add(evLayer);
                        m_EventElements[eid] = evLayer;
                    }
                }
            }

            m_Map.ViewChangeEnd += (s, e) =>
                {
                    foreach (MapLayer mapLayer in m_MapLayers.Values)
                        mapLayer.Visibility = Visibility.Hidden;

                    if (m_Map.ZoomLevel >= 3)
                    {
                        int mid = GetMapByCenter(m_Map.Project(m_Map.Center, m_Map.MaxZoomLevel));
                        if (m_MapLayers.ContainsKey(mid))
                        {
                            m_MapLayers[mid].Visibility = Visibility.Visible;
                        }
                    }
                };

            m_Running = true;
            m_Canceled = new ManualResetEvent(false);
            m_PlayerWorkerThread = new Thread(PlayerWorkerThread);
            m_PlayerWorkerThread.Start();
            m_EventWorkerThread = new Thread(EventWorkerThread);
            m_EventWorkerThread.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            m_Running = false;
            m_Canceled.Set();
            m_PlayerWorkerThread.Join();
            m_EventWorkerThread.Join();

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

        private void PlayerWorkerThread()
        {
            while (m_Running)
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

                m_Canceled.WaitOne(100);
            }
        }

        private void EventWorkerThread()
        {
            while (m_Running)
            {
                try
                {
                    EventsResponse events = new EventsRequest(1007).Execute();

                    Dispatcher.Invoke(() =>
                        {
                            foreach (EventState ev in events.Events)
                            {
                                if (m_EventElements.ContainsKey(ev.EventId))
                                {
                                    m_EventElements[ev.EventId].SetEventState(ev.StateEnum);
                                }
                            }
                        }, DispatcherPriority.Render, new CancellationToken(), new TimeSpan(0, 0, 25));
                }
                catch
                { }

                m_Canceled.WaitOne(30000);
            }
        }
    }
}
