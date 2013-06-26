using System;
using System.Collections.Generic;
using System.Windows;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetMap : Map
    {
        public double MinZoomLevel { get { return 1; } }
        public double MaxZoomLevel { get { return 7; } }

        public ArenaNetMap()
        {
            AnimationLevel = Microsoft.Maps.MapControl.WPF.AnimationLevel.Full;
            ScaleVisibility = System.Windows.Visibility.Hidden;

            ViewChangeEnd += (s, e) =>
                {
                    if (ZoomLevel > MaxZoomLevel)
                        SetView(MaxZoomLevel, Heading);
                    else if (ZoomLevel < MinZoomLevel)
                        SetView(MinZoomLevel, Heading);
                };

            Mode = new MercatorMode();
            Children.Add(new ArenaNetTileLayer());
        }

        public Location Unproject(Point p, double zoomLevel)
        {
            double mapSize = MapSize((int)Math.Round(zoomLevel));
            double x = (ClipBounds(p.X, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (ClipBounds(p.Y, 0, mapSize - 1) / mapSize);

            double lat = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            double lng = 360 * x;

            return new Location(lat, lng);
        }

        public Point Project(Location l, double zoomLevel)
        {
            double lat = ClipBounds(l.Latitude, Location.MinLatitude, Location.MaxLatitude);
            double lng = ClipBounds(l.Longitude, Location.MinLongitude, Location.MaxLongitude);

            double x = (lng + 180) / 360;
            double sLat = Math.Sin(lat * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sLat) / (1 - sLat)) / (4 * Math.PI);

            double mapSize = MapSize((int)Math.Round(zoomLevel));
            int px = (int)ClipBounds(x * mapSize + 0.5, 0, mapSize - 1);
            int py = (int)ClipBounds(y * mapSize + 0.5, 0, mapSize - 1);

            return new Point(px, py);
        }

        private double ClipBounds(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        private uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }
    }
}
