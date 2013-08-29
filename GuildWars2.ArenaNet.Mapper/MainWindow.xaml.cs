using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;
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
using Location = Microsoft.Maps.MapControl.WPF.Location;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.API;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MumbleLink m_Link;
        private IDictionary<int, FloorMapDetails> m_MapData;

        private volatile bool m_Running;
        private ManualResetEvent m_Canceled;
        private Thread m_PlayerWorkerThread;
        private Thread m_EventWorkerThread;
        private Thread m_EventTimerWorkerThread;

        // map stuff
        private volatile bool m_FollowPlayer;
        private PlayerPushpin m_Player;

        private ArenaNetMapLayerContainer m_MapLayerContainer;

        // event timer stuff
        private IDictionary<string, EventTimerBox> m_EventTimerBoxes;

        public MainWindow()
        {
            m_MapLayerContainer = new ArenaNetMapLayerContainer();

            InitializeComponent();

            m_Link = new MumbleLink();
            m_MapData = new Dictionary<int, FloorMapDetails>();

            m_FollowPlayer = false;
            m_Player = new PlayerPushpin();
            m_Player.Template = (ControlTemplate)Application.Current.Resources["PlayerPushpin"];
            m_Player.PositionOrigin = PositionOrigin.Center;
            m_Player.Visibility = Visibility.Hidden;
            Map.Children.Add(m_Player);

            Map.Children.Add(m_MapLayerContainer);

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
                        m_MapLayerContainer.LoadFloorMapDetails(mid, map);
                    }
                }
            }

            m_MapLayerContainer.LoadBounties();

            EventDetailsResponse events = new EventDetailsRequest().Execute();
            if (events != null)
            {
                IList<Guid> championEvents = new ChampionEventsRequest().Execute();

                foreach (KeyValuePair<string, EventDetails> entry in events.Events)
                {
                    Guid eid = new Guid(entry.Key);
                    EventDetails ev = entry.Value;

                    if (!ev.Name.StartsWith("skill challenge: ", StringComparison.InvariantCultureIgnoreCase) && m_MapData.ContainsKey(ev.MapId))
                    {
                        FloorMapDetails map = m_MapData[ev.MapId];

                        m_MapLayerContainer.LoadEvent(eid, ev, map, championEvents.Contains(eid));
                    }
                }
            }

            m_EventTimerBoxes = new Dictionary<string, EventTimerBox>();
            EventTimerDataResponse timerData = new EventTimerDataRequest().Execute();
            if (timerData != null)
            {
                foreach (MetaEventStatus e in timerData.Events)
                {
                    EventTimerBox box = new EventTimerBox();
                    box.SetData(e);
                    m_EventTimerBoxes.Add(e.Id, box);

                    switch (e.StageTypeEnum)
                    {
                        case MetaEventStage.StageType.Boss:
                            EventTimerItems_Boss.Children.Add(box);
                            break;
                        case MetaEventStage.StageType.PreEvent:
                            EventTimerItems_PreEvent.Children.Add(box);
                            break;
                        default:
                            EventTimerItems_Other.Children.Add(box);
                            break;
                    }
                }
            }

            MouseDown += DrawPolyMouseDownHandler;
            KeyDown += DrawPolyKeyDownHandler;
            KeyUp += DrawPolyKeyUpHandler;

            m_Running = true;
            m_Canceled = new ManualResetEvent(false);
            m_PlayerWorkerThread = new Thread(PlayerWorkerThread);
            m_PlayerWorkerThread.Start();
            m_EventWorkerThread = new Thread(EventWorkerThread);
            m_EventWorkerThread.Start();
            m_EventTimerWorkerThread = new Thread(EventTimerWorkerThread);
            m_EventTimerWorkerThread.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            m_Running = false;
            m_Canceled.Set();
            m_PlayerWorkerThread.Join();
            m_EventWorkerThread.Join();
            m_EventTimerWorkerThread.Join();

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

        #region Worker Threads
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

                        posX = ArenaNetMap.TranslateX(posX, map.MapRect, map.ContinentRect);
                        posZ = ArenaNetMap.TranslateZ(posZ, map.MapRect, map.ContinentRect);

                        Location loc = ArenaNetMap.Unproject(new Point(posX, posZ), ArenaNetMap.MaxZoomLevel);

                        // move the player icon
                        Dispatcher.Invoke(() =>
                            {
                                m_Player.Heading = rot;
                                m_Player.Visibility = Visibility.Visible;

                                // only follow player if they've asked to and the location has changed
                                if (m_FollowPlayer && m_Player.Location != loc)
                                    Map.SetView(loc, Map.ZoomLevel);

                                m_Player.Location = loc;
                            }, DispatcherPriority.Background, CancellationToken.None, new TimeSpan(0, 0, 1));
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
                                m_MapLayerContainer.SetEventState(ev.EventId, ev.StateEnum);
                            }
                        }, DispatcherPriority.Background, CancellationToken.None, new TimeSpan(0, 0, 25));
                }
                catch
                { }

                m_Canceled.WaitOne(30000);
            }
        }

        private void EventTimerWorkerThread()
        {

            Timer ticker = new Timer();
            ticker.Interval = 1000;
            ticker.Elapsed += (s, e) =>
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                            {
                                foreach (EventTimerBox b in m_EventTimerBoxes.Values)
                                    b.Tick();
                            }, DispatcherPriority.Background, CancellationToken.None, new TimeSpan(0, 0, 1));
                    }
                    catch
                    { }
                };
            ticker.Start();

            while (m_Running)
            {
                try
                {
                    EventTimerDataResponse timerData = new EventTimerDataRequest().Execute();
                    if (timerData != null)
                    {
                        Dispatcher.Invoke(() =>
                            {
                                EventTimerItems_Boss.Children.Clear();
                                EventTimerItems_PreEvent.Children.Clear();
                                EventTimerItems_Other.Children.Clear();

                                foreach (MetaEventStatus e in timerData.Events)
                                {
                                    EventTimerBox box = m_EventTimerBoxes[e.Id];
                                    box.SetData(e);

                                    switch (e.StageTypeEnum)
                                    {
                                        case MetaEventStage.StageType.Boss:
                                            EventTimerItems_Boss.Children.Add(box);
                                            break;
                                        case MetaEventStage.StageType.PreEvent:
                                            EventTimerItems_PreEvent.Children.Add(box);
                                            break;
                                        default:
                                            EventTimerItems_Other.Children.Add(box);
                                            break;
                                    }
                                }
                            }, DispatcherPriority.Background, CancellationToken.None, new TimeSpan(0, 0, 25));
                    }
                }
                catch
                { }

                m_Canceled.WaitOne(30000);
            }

            ticker.Stop();
        }
        #endregion

        #region Map Handlers
        private void Map_UpdateView(double zoomLevel)
        {
            m_MapLayerContainer.HideMapLayer();

            if (zoomLevel >= 3)
            {
                int mid = GetMapByCenter(ArenaNetMap.Project(Map.Center, ArenaNetMap.MaxZoomLevel));
                m_MapLayerContainer.ShowMapLayer(mid);
            }

            ZoomInButton.IsEnabled = (zoomLevel < ArenaNetMap.MaxZoomLevel);
            ZoomOutButton.IsEnabled = (zoomLevel > ArenaNetMap.MinZoomLevel);
        }

        private void Map_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            Map_UpdateView(Map.TargetZoomLevel);
        }

        private void Map_ViewChangeEnd(object sender, MapEventArgs e)
        {
            Map_UpdateView(Map.ZoomLevel);
        }
        #endregion

        #region Legend Checkbox Handlers
        private void LegendIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            LegendIcon.Visibility = Visibility.Collapsed;
            Legend.Visibility = Visibility.Visible;
        }

        private void Legend_MouseLeave(object sender, MouseEventArgs e)
        {
            Legend.Visibility = Visibility.Collapsed;
            LegendIcon.Visibility = Visibility.Visible;
        }

        private void Legend_WaypointsChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowWaypoints(true); }
        private void Legend_WaypointsUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowWaypoints(false); }

        private void Legend_PointsOfInterestChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowPointsOfInterest(true); }
        private void Legend_PointsOfInterestUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowPointsOfInterest(false); }

        private void Legend_VistasChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowVistas(true); }
        private void Legend_VistasUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowVistas(false); }

        private void Legend_RenownHeartsChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowRenownHearts(true); }
        private void Legend_RenownHeartsUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowRenownHearts(false); }

        private void Legend_SkillPointsChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowSkillPoints(true); }
        private void Legend_SkillPointsUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowSkillPoints(false); }

        private void Legend_SectorsChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowSectors(true); }
        private void Legend_SectorsUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowSectors(false); }

        private void Legend_BountiesChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowBounties(true); }
        private void Legend_BountiesUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowBounties(false); }

        private void Legend_EventsChecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowEvents(true); }
        private void Legend_EventsUnchecked(object sender, RoutedEventArgs e) { m_MapLayerContainer.ShowEvents(false); }

        private void Legend_FollowPlayerChecked(object sender, RoutedEventArgs e) { m_FollowPlayer = true; }
        private void Legend_FollowPlayerUnchecked(object sender, RoutedEventArgs e) { m_FollowPlayer = false; }
        #endregion

        #region Zoom Handlers
        private void ZoomInButton_Clicked(object sender, RoutedEventArgs e)
        {
            double newZoomLevel = Math.Min(Map.ZoomLevel + 1.0, ArenaNetMap.MaxZoomLevel);

            if (newZoomLevel != Map.ZoomLevel)
                Map.SetView(newZoomLevel, Map.Heading);
        }

        private void ZoomOutButton_Clicked(object sender, RoutedEventArgs e)
        {
            double newZoomLevel = Math.Max(Map.ZoomLevel - 1.0, ArenaNetMap.MinZoomLevel);

            if (newZoomLevel != Map.ZoomLevel)
                Map.SetView(newZoomLevel, Map.Heading);
        }
        #endregion

        #region Event Timer Handlers
        private void EventTimerIcon_MouseDown(object sender, MouseEventArgs e)
        {
            EventTimerItems.Visibility = (EventTimerItems.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
        #endregion

        #region DrawPolyHandlers
        private bool m_DrawingPolygon = false;
        private MapPolygon m_DrawingPolygonItem = null;
        private MapPolyline m_DrawingPolylineItem = null;
        private void DrawPolyKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl && !e.IsRepeat && !m_DrawingPolygon)
            {
                m_DrawingPolygon = true;

                if (m_DrawingPolygonItem != null)
                    Map.Children.Remove(m_DrawingPolygonItem);

                m_DrawingPolygonItem = null;
                
                m_DrawingPolylineItem = new MapPolyline();
                m_DrawingPolylineItem.Locations = new LocationCollection();
                m_DrawingPolylineItem.Opacity = 0.7;
                m_DrawingPolylineItem.Stroke = System.Windows.Media.Brushes.Blue;
                m_DrawingPolylineItem.StrokeThickness = 3;

                Map.Children.Add(m_DrawingPolylineItem);

                e.Handled = true;
            }

            if (e.Key == Key.RightCtrl && !e.IsRepeat && m_DrawingPolygonItem != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("double[,] points = new double[,] {");

                for (int i = 0, n= m_DrawingPolygonItem.Locations.Count; i < n; i++)
                {
                    Point p = ArenaNetMap.Project(m_DrawingPolygonItem.Locations[i], ArenaNetMap.MaxZoomLevel);
                    sb.AppendFormat("{0}{{{1}, {2}}}", (i == 0 ? string.Empty : ", "), p.X, p.Y);
                }

                sb.Append("};");

                Window textBoxWindow = new Window();
                textBoxWindow.Width = 320;
                textBoxWindow.Height = 240;

                TextBox textBox = new TextBox();
                textBox.Text = sb.ToString();
                textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                textBoxWindow.Content = textBox;

                textBoxWindow.Show();
            }
        }
        private void DrawPolyKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl && m_DrawingPolygon)
            {
                m_DrawingPolygon = false;

                m_DrawingPolygonItem = new MapPolygon();
                m_DrawingPolygonItem.Locations = m_DrawingPolylineItem.Locations;
                m_DrawingPolygonItem.Opacity = m_DrawingPolylineItem.Opacity;
                m_DrawingPolygonItem.Stroke = m_DrawingPolylineItem.Stroke;
                m_DrawingPolygonItem.StrokeThickness = m_DrawingPolylineItem.StrokeThickness;

                Map.Children.Add(m_DrawingPolygonItem);

                Map.Children.Remove(m_DrawingPolylineItem);
                m_DrawingPolylineItem = null;

                e.Handled = true;
            }
        }
        private void DrawPolyMouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && m_DrawingPolygon && m_DrawingPolylineItem != null)
            {
                m_DrawingPolylineItem.Locations.Add(Map.ViewportPointToLocation(e.GetPosition(Map)));

                e.Handled = true;
            }
        }
        #endregion
    }
}
