using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
using Location = Microsoft.Maps.MapControl.Location;
#else
using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;
#endif

using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class BountyMapLayer : MapLayer
    {
        public string BountyName { get; set; }

        public BountyMapLayer(string name)
            : base()
        {
            BountyName = name;
        }

        public void AddSpawningPoint(Location loc)
        {
            BountyPushpin pin = new BountyPushpin();
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

                    BountyPushpin pin = new BountyPushpin();
                    pin.ToolTip = string.Format("{0} ({1}Path)", BountyName,
                            (direction == PathDirectionType.Invalid ? string.Empty : Enum.GetName(typeof(PathDirectionType), direction) + " "));
                    pin.Location = loc;
                    Children.Add(pin);
                }
            }
        }
    }
}
