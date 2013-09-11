using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#if SILVERLIGHT
using Color = System.Windows.Media.Color;

using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

using GuildWars2.SyntaxError.API;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetMapLayerContainer : MapLayer
    {
        private static SolidColorBrush[] BOUNTY_PATH_BRUSHES = new SolidColorBrush[] {
#if SILVERLIGHT
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.White),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x32, 0xCD, 0x32))
#else
                Brushes.Blue, Brushes.White, Brushes.Yellow, Brushes.LimeGreen
#endif
            };

        private IDictionary<int, MapLayer> m_MapLayers;

        private IDictionary<int, MapLayer> m_MapWaypoints;
        private IDictionary<int, MapLayer> m_MapPointsOfInterest;
        private IDictionary<int, MapLayer> m_MapVistas;
        private IDictionary<int, MapLayer> m_MapRenownHearts;
        private IDictionary<int, MapLayer> m_MapSkillPoints;
        private IDictionary<int, MapLayer> m_MapSectors;
        
        private IDictionary<int, MapLayer> m_MapBounties;

        private IDictionary<int, MapLayer> m_MapEvents;
        private IDictionary<int, MapLayer> m_MapEventPolygons;
        private IDictionary<Guid, EventPushpin> m_EventPushpins;
        private IDictionary<Guid, EventMapPolygon> m_EventMapPolygons;

        public ArenaNetMapLayerContainer()
            : base()
        {
            m_MapLayers = new Dictionary<int, MapLayer>();
            m_MapWaypoints = new Dictionary<int, MapLayer>();
            m_MapPointsOfInterest = new Dictionary<int, MapLayer>();
            m_MapVistas = new Dictionary<int, MapLayer>();
            m_MapRenownHearts = new Dictionary<int, MapLayer>();
            m_MapSkillPoints = new Dictionary<int, MapLayer>();
            m_MapSectors = new Dictionary<int, MapLayer>();

            m_MapBounties = new Dictionary<int, MapLayer>();

            m_MapEvents = new Dictionary<int, MapLayer>();
            m_MapEventPolygons = new Dictionary<int, MapLayer>();
            m_EventPushpins = new Dictionary<Guid, EventPushpin>();
            m_EventMapPolygons = new Dictionary<Guid, EventMapPolygon>();
        }

        #region Load Methods
        public void LoadFloorMapDetails(int mid, FloorMapDetails map)
        {
            if (!m_MapLayers.ContainsKey(mid))
            {
                m_MapLayers.Add(mid, new MapLayer());
            }

            if (!m_MapWaypoints.ContainsKey(mid))
            {
                m_MapWaypoints.Add(mid, new MapLayer());
                m_MapLayers[mid].Children.Add(m_MapWaypoints[mid]);
            }

            if (!m_MapPointsOfInterest.ContainsKey(mid))
            {
                m_MapPointsOfInterest.Add(mid, new MapLayer());
                m_MapLayers[mid].Children.Add(m_MapPointsOfInterest[mid]);
            }

            if (!m_MapVistas.ContainsKey(mid))
            {
                m_MapVistas.Add(mid, new MapLayer());
                m_MapLayers[mid].Children.Add(m_MapVistas[mid]);
            }

            if (!m_MapRenownHearts.ContainsKey(mid))
            {
                m_MapRenownHearts.Add(mid, new MapLayer());
                m_MapLayers[mid].Children.Add(m_MapRenownHearts[mid]);
            }

            if (!m_MapSkillPoints.ContainsKey(mid))
            {
                m_MapSkillPoints.Add(mid, new MapLayer());
                m_MapLayers[mid].Children.Add(m_MapSkillPoints[mid]);
            }

            if (!m_MapSectors.ContainsKey(mid))
            {
                m_MapSectors.Add(mid, new MapLayer());
                m_MapLayers[mid].Children.Add(m_MapSectors[mid]);
            }

            m_MapWaypoints[mid].Children.Clear();
            m_MapPointsOfInterest[mid].Children.Clear();
            m_MapVistas[mid].Children.Clear();
            m_MapRenownHearts[mid].Children.Clear();
            m_MapSkillPoints[mid].Children.Clear();
            m_MapSectors[mid].Children.Clear();

            foreach (PointOfInterest poi in map.PointsOfInterest)
            {
                Pushpin poiPin = new PointOfInterestPushpin(poi);

                poiPin.Location = ArenaNetMap.Unproject(new Point(poi.Coord[0], poi.Coord[1]), ArenaNetMap.MaxZoomLevel);

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

                rhPin.Location = ArenaNetMap.Unproject(new Point(rh.Coord[0], rh.Coord[1]), ArenaNetMap.MaxZoomLevel);

                m_MapRenownHearts[mid].Children.Add(rhPin);
            }

            foreach (MappedModel sp in map.SkillChallenges)
            {
                Pushpin spPin = new SkillChallengePushpin(sp);

                spPin.Location = ArenaNetMap.Unproject(new Point(sp.Coord[0], sp.Coord[1]), ArenaNetMap.MaxZoomLevel);

                m_MapSkillPoints[mid].Children.Add(spPin);
            }

            // hide sectors by default
            m_MapSectors[mid].Visibility = Visibility.Collapsed;
            foreach (Sector s in map.Sectors)
            {
                Pushpin sPin = new SectorPushpin(s);

                sPin.Location = ArenaNetMap.Unproject(new Point(s.Coord[0], s.Coord[1]), ArenaNetMap.MaxZoomLevel);

                m_MapSectors[mid].Children.Add(sPin);
            }
        }

        public void LoadBounties()
        {
            // clear the bounties
            foreach (MapLayer map in m_MapBounties.Values)
                map.Children.Clear();

            foreach (GuildBounty bounty in GuildBountyDefinitions.BOUNTIES)
            {
                int mid = bounty.MapId;

                if (!m_MapLayers.ContainsKey(mid))
                {
                    m_MapLayers.Add(mid, new MapLayer());
                }

                BountyMapLayer b = new BountyMapLayer(bounty.Name);

                if (!m_MapBounties.ContainsKey(mid))
                {
                    m_MapBounties.Add(mid, new MapLayer());

                    // map bounties default to hidden
                    m_MapBounties[mid].Visibility = Visibility.Collapsed;

                    // we insert instead of add so events always show up under other pushpins
                    m_MapLayers[mid].Children.Insert(0, m_MapBounties[mid]);
                }

                if (bounty.Spawns != null)
                {
                    foreach (List<double> p in bounty.Spawns)
                        b.AddSpawningPoint(ArenaNetMap.Unproject(new Point(p[0], p[1]), ArenaNetMap.MaxZoomLevel));
                }

                if (bounty.Paths != null)
                {
                    int i = 0;
                    foreach (GuildBountyPath path in bounty.Paths)
                    {
                        LocationCollection locs = new LocationCollection();
                        foreach (List<double> p in path.Points)
                            locs.Add(ArenaNetMap.Unproject(new Point(p[0], p[1]), ArenaNetMap.MaxZoomLevel));
                        b.AddPath(locs, BOUNTY_PATH_BRUSHES[i], path.Direction);
                        i = (i + 1) % BOUNTY_PATH_BRUSHES.Length;
                    }
                }

                m_MapBounties[mid].Children.Add(b);
            }
        }

        public void LoadEvent(Guid eid, EventDetails ev, FloorMapDetails map, bool isChampion)
        {
            int mid = ev.MapId;

            if (!m_MapLayers.ContainsKey(mid))
            {
                m_MapLayers.Add(mid, new MapLayer());
            }

            if (!m_MapEvents.ContainsKey(mid))
            {
                m_MapEvents.Add(mid, new MapLayer());

                // we insert instead of add so events always show up under other pushpins
                m_MapLayers[mid].Children.Insert(0, m_MapEvents[mid]);
            }

            if (!m_MapEventPolygons.ContainsKey(mid))
            {
                m_MapEventPolygons.Add(mid, new MapLayer());

                // we insert instead of add so events always show up under other pushpins
                m_MapLayers[mid].Children.Insert(0, m_MapEventPolygons[mid]);
            }

            // clean up
            if (m_EventMapPolygons.ContainsKey(eid))
            {
                m_MapEvents[mid].Children.Remove(m_EventMapPolygons[eid]);
                m_EventMapPolygons.Remove(eid);
            }

            if (m_EventPushpins.ContainsKey(eid))
            {
                m_MapEvents[mid].Children.Remove(m_EventPushpins[eid]);
                m_EventPushpins.Remove(eid);
            }

            Point center = new Point(ArenaNetMap.TranslateX(ev.Location.Center[0], map.MapRect, map.ContinentRect),
                        ArenaNetMap.TranslateZ(ev.Location.Center[1], map.MapRect, map.ContinentRect));

            switch (ev.Location.TypeEnum)
            {
                case LocationType.Poly:
                    EventMapPolygon evPoly = new EventMapPolygon(ev, isChampion);

                    foreach (List<double> pt in ev.Location.Points)
                    {
                        evPoly.Locations.Add(
                                ArenaNetMap.Unproject(
                                        new Point(
                                                ArenaNetMap.TranslateX(pt[0], map.MapRect, map.ContinentRect),
                                                ArenaNetMap.TranslateZ(pt[1], map.MapRect, map.ContinentRect)),
                                        ArenaNetMap.MaxZoomLevel));
                    }

                    m_EventMapPolygons[eid] = evPoly;
                    // insert so polys are below all pushpins
                    m_MapEventPolygons[mid].Children.Insert(0, evPoly);
                    break;

                case LocationType.Sphere:
                case LocationType.Cylinder:
                    EventMapPolygon evCircle = new EventMapPolygon(ev, isChampion);

                    double radius = ArenaNetMap.TranslateX(ev.Location.Center[0] + ev.Location.Radius, map.MapRect, map.ContinentRect) - center.X;

                    for (int i = 0; i < 360; i += 10)
                    {
                        evCircle.Locations.Add(
                                ArenaNetMap.Unproject(
                                        new Point(
                                                center.X + radius * Math.Cos(i * (Math.PI / 180)),
                                                center.Y + radius * Math.Sin(i * (Math.PI / 180))),
                                        ArenaNetMap.MaxZoomLevel));
                    }

                    m_EventMapPolygons[eid] = evCircle;
                    // insert so polys are below all pushpins
                    m_MapEventPolygons[ev.MapId].Children.Insert(0, evCircle);
                    break;

                default:
                    break;
            }

            EventPushpin evPin = new EventPushpin(ev);
            evPin.Location = ArenaNetMap.Unproject(center, ArenaNetMap.MaxZoomLevel);
            m_EventPushpins[eid] = evPin;
            m_MapEvents[ev.MapId].Children.Add(evPin);
        }
        #endregion

        #region Update Methods
        public void SetEventState(Guid eid, EventStateType es)
        {
            if (m_EventPushpins.ContainsKey(eid))
                m_EventPushpins[eid].SetEventState(es);

            if (m_EventMapPolygons.ContainsKey(eid))
                m_EventMapPolygons[eid].SetEventState(es);
        }
        #endregion

        #region Show / Hide Methods
        private void SetMapLayerVisibility(IDictionary<int, MapLayer> layerDict, Visibility visibility)
        {
            if (layerDict == null)
                return;

            foreach (MapLayer layer in layerDict.Values)
                layer.Visibility = visibility;
        }

        public void HideMapLayer(int mid = -1)
        {
            if (mid < 0)
                Children.Clear();
            else
            {
                if (m_MapLayers.ContainsKey(mid))
                    Children.Remove(m_MapLayers[mid]);
            }
        }

        public void ShowMapLayer(int mid)
        {
            if (m_MapLayers.ContainsKey(mid))
                Children.Add(m_MapLayers[mid]);
        }

        public void ShowWaypoints(bool visible) { SetMapLayerVisibility(m_MapWaypoints, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowPointsOfInterest(bool visible) { SetMapLayerVisibility(m_MapPointsOfInterest, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowVistas(bool visible) { SetMapLayerVisibility(m_MapVistas, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowRenownHearts(bool visible) { SetMapLayerVisibility(m_MapRenownHearts, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowSkillPoints(bool visible) { SetMapLayerVisibility(m_MapSkillPoints, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowSectors(bool visible) { SetMapLayerVisibility(m_MapSectors, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowBounties(bool visible) { SetMapLayerVisibility(m_MapBounties, (visible ? Visibility.Visible : Visibility.Collapsed)); }
        public void ShowEvents(bool visible)
        {
            SetMapLayerVisibility(m_MapEvents, (visible ? Visibility.Visible : Visibility.Collapsed));
            SetMapLayerVisibility(m_MapEventPolygons, (visible ? Visibility.Visible : Visibility.Collapsed));
        }
        #endregion
    }
}
