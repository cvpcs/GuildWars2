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

using Microsoft.Maps.MapControl;
using Location = Microsoft.Maps.MapControl.Location;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.API;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        private ManualResetEvent m_MapDataLoaded = new ManualResetEvent(false);
        private ManualResetEvent m_EventDataLoaded = new ManualResetEvent(false);
        private IDictionary<int, FloorMapDetails> m_MapData;

        private volatile bool m_Running;
        private ManualResetEvent m_Canceled;
        private Thread m_EventWorkerThread;

        private ArenaNetMapLayerContainer m_MapLayerContainer;

        public MainPage()
        {
            m_MapLayerContainer = new ArenaNetMapLayerContainer();

            InitializeComponent();

            m_MapData = new Dictionary<int, FloorMapDetails>();

            Map.Children.Add(m_MapLayerContainer);

            new MapFloorRequest(1, 2).ExecuteAsync(floor =>
                {
                    if (floor != null)
                    {
                        foreach (FloorRegion region in floor.Regions.Values)
                        {
                            foreach (string mapId in region.Maps.Keys)
                            {
                                int mid = int.Parse(mapId);
                                FloorMapDetails map = region.Maps[mapId];
                                m_MapData.Add(mid, map);
                            }
                        }

                        m_MapDataLoaded.Set();

                        Dispatcher.BeginInvoke(() =>
                            {
                                foreach (KeyValuePair<int, FloorMapDetails> entry in m_MapData)
                                {
                                    m_MapLayerContainer.LoadFloorMapDetails(entry.Key, entry.Value);
                                }
                            });
                    }
                });

            m_MapLayerContainer.LoadBounties();

            new EventDetailsRequest().ExecuteAsync(events =>
                {
                    if (events != null)
                    {
                        new ChampionEventsRequest().ExecuteAsync(championEvents =>
                            {
                                IDictionary<Guid, EventDetails> evDetails = new Dictionary<Guid, EventDetails>();
                                IDictionary<Guid, bool> evChamps = new Dictionary<Guid, bool>();

                                foreach (KeyValuePair<string, EventDetails> entry in events.Events)
                                {
                                    Guid eid = new Guid(entry.Key);
                                    EventDetails ev = entry.Value;
                                    if (!ev.Name.StartsWith("skill challenge: ", StringComparison.InvariantCultureIgnoreCase) && m_MapData.ContainsKey(ev.MapId))
                                    {
                                        evDetails[eid] = ev;
                                        evChamps[eid] = championEvents.Contains(eid);
                                    }
                                }

                                // ensure map data is loaded
                                m_MapDataLoaded.WaitOne();

                                Dispatcher.BeginInvoke(() =>
                                    {
                                        foreach (KeyValuePair<Guid, EventDetails> entry in evDetails)
                                        {
                                            m_MapLayerContainer.LoadEvent(entry.Key, entry.Value, m_MapData[entry.Value.MapId], evChamps[entry.Key]);
                                        }

                                        m_EventDataLoaded.Set();
                                    });
                            });
                    }
                });
        }

        public void StartThreads()
        {
            if (!m_Running)
            {
                m_Running = true;
                m_Canceled = new ManualResetEvent(false);
                m_EventWorkerThread = new Thread(EventWorkerThread);
                m_EventWorkerThread.Start();
            }
        }

        public void StopThreads()
        {
            if (m_Running)
            {
                m_Running = false;
                m_Canceled.Set();
                m_EventWorkerThread.Join();
            }
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
        private void EventWorkerThread()
        {
            // wait for event data to be loaded before proceeding (or if we are cancelled)
            EventWaitHandle.WaitAny(new ManualResetEvent[] { m_EventDataLoaded, m_Canceled });

            while (m_Running)
            {
                try
                {
                    new EventsRequest(1007).ExecuteAsync(events =>
                        {
                            Dispatcher.BeginInvoke(() =>
                                {
                                    foreach (EventState ev in events.Events)
                                    {
                                        m_MapLayerContainer.SetEventState(ev.EventId, ev.StateEnum);
                                    }
                                });
                        });
                }
                catch
                { }

                m_Canceled.WaitOne(30000);
            }
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
        #endregion

        #region Zoom Handlers
        private void ZoomInButton_Clicked(object sender, RoutedEventArgs e)
        {
            double newZoomLevel = Math.Min(Map.ZoomLevel + 1.0, ArenaNetMap.MaxZoomLevel);

            if (newZoomLevel != Map.ZoomLevel)
                Map.SetView(Map.Center, newZoomLevel);
        }

        private void ZoomOutButton_Clicked(object sender, RoutedEventArgs e)
        {
            double newZoomLevel = Math.Max(Map.ZoomLevel - 1.0, ArenaNetMap.MinZoomLevel);

            if (newZoomLevel != Map.ZoomLevel)
                Map.SetView(Map.Center, newZoomLevel);
        }
        #endregion
    }
}
