using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;

using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class BountyMapLayer : MapLayer
    {
        private static BitmapImage IMAGE;

        static BountyMapLayer()
        {
            IMAGE = new BitmapImage();
            IMAGE.BeginInit();
            IMAGE.StreamSource = Application.GetResourceStream(new Uri("/Resources/bounty.png", UriKind.Relative)).Stream;
            IMAGE.EndInit();
        }

        public string BountyName { get; set; }

        public BountyMapLayer(string name)
            : base()
        {
            BountyName = name;
        }

        public void AddSpawningPoint(Location loc)
        {
            ImagePushpin pin = new ImagePushpin();
            pin.Image = IMAGE;
            pin.Width = 20;
            pin.Height = 20;
            pin.PositionOrigin = PositionOrigin.Center;
            pin.ToolTip = string.Format("{0} (Spawning Point)", BountyName);
            pin.Location = loc;
            Children.Add(pin);
        }

        public void AddSpawningPoints(LocationCollection locs)
        {
            foreach (Location loc in locs)
                AddSpawningPoint(loc);
        }

        public void AddPath(LocationCollection polyPoints, SolidColorBrush brush, PathDirectionType direction)
        {
            MapPolygon poly = new MapPolygon();
            poly.Opacity = 0.8;
            poly.StrokeThickness = 3;
            poly.Stroke = brush;
            poly.Locations = polyPoints;
            Children.Add(poly);

            int numPoints = 1;
            while (numPoints * 10 < polyPoints.Count)
                numPoints *= 2;

            for (int i = 0; i < numPoints; i++)
            {
                int j = i * (polyPoints.Count / numPoints);

                if (j < polyPoints.Count)
                {
                    Location loc = polyPoints[j];

                    ImagePushpin pin = new ImagePushpin();
                    pin.Image = IMAGE;
                    pin.Width = 20;
                    pin.Height = 20;
                    pin.PositionOrigin = PositionOrigin.Center;
                    pin.ToolTip = string.Format("{0} ({1}Path)", BountyName,
                            (direction == PathDirectionType.Invalid ? string.Empty : Enum.GetName(typeof(PathDirectionType), direction) + " "));
                    pin.Location = loc;
                    Children.Add(pin);
                }
            }
        }
    }
}
