using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetMap : Map
    {
        public static double MinZoomLevel { get { return 1; } }
        public static double MaxZoomLevel { get { return 7; } }

        public ArenaNetMap()
        {
            AnimationLevel = AnimationLevel.Full;
            ScaleVisibility = Visibility.Collapsed;

#if SILVERLIGHT
            NavigationVisibility = Visibility.Collapsed;
#endif

            MouseWheel += MouseWheelHandler;
            MouseDoubleClick += MouseDoubleClickHandler;

            ViewChangeEnd += ViewChangeEndHandler;

            Mode = new RoadMode();
            Children.Add(new ArenaNetTileLayer());
        }

        public static Location Unproject(Point p, double zoomLevel)
        {
            double mapSize = MapSize((int)Math.Round(zoomLevel));
            double x = (ClipBounds(p.X, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (ClipBounds(p.Y, 0, mapSize - 1) / mapSize);

            double lat = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            double lng = 360 * x;

            return new Location(lat, lng);
        }

        public static Point Project(Location l, double zoomLevel)
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

        public static double TranslateX(double x, List<List<double>> mapRect, List<List<double>> continentRect)
        {
            return (x - mapRect[0][0]) / (mapRect[1][0] - mapRect[0][0]) * (continentRect[1][0] - continentRect[0][0]) + continentRect[0][0];
        }

        public static double TranslateZ(double z, List<List<double>> mapRect, List<List<double>> continentRect)
        {
            return (1 - ((z - mapRect[0][1]) / (mapRect[1][1] - mapRect[0][1]))) * (continentRect[1][1] - continentRect[0][1]) + continentRect[0][1];
        }

        private static double ClipBounds(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        private static uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }

        private void ZoomAboutPoint(Point point, double zoomLevelIncrement)
        {
            double newZoomLevel = ClipBounds(TargetZoomLevel + zoomLevelIncrement, MinZoomLevel, MaxZoomLevel);
            double newZoomLevelIncrement = newZoomLevel - TargetZoomLevel;

            if (newZoomLevelIncrement != 0)
            {
                Point center = LocationToViewportPoint(Center);

                Location zoomTo;
                if (zoomLevelIncrement > 0)
                {
                    zoomTo = ViewportPointToLocation(new Point(
                            (point.X + center.X)/2,
                            (point.Y + center.Y)/2));
                }
                else
                {
                    zoomTo = ViewportPointToLocation(new Point(
                            2 * newZoomLevelIncrement * (point.X - center.X) + center.X,
                            2 * newZoomLevelIncrement * (point.Y - center.Y) + center.Y
                        ));
                }

                SetView(zoomTo, newZoomLevel);
            }
        }

        #region Event Handling
        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            ZoomAboutPoint(e.GetPosition(this), ((double)e.Delta / 100.0));
            e.Handled = true;
        }

#if SILVERLIGHT
        private void MouseDoubleClickHandler(object sender, MapMouseEventArgs e)
        {
            ZoomAboutPoint(e.ViewportPoint, 1.0);
            e.Handled = true;
        }
#else
        private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            ZoomAboutPoint(e.GetPosition(this), 1.0);
            e.Handled = true;
        }
#endif

        private void ViewChangeEndHandler(object sender, MapEventArgs e)
        {
            double newZoomLevel = ClipBounds(ZoomLevel, MinZoomLevel, MaxZoomLevel);
            if (newZoomLevel != ZoomLevel)
                SetView(Center, newZoomLevel);
        }
        #endregion
    }
}
