using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class PointOfInterestPushpin : ImagePushpin
    {
        private static IDictionary<PointOfInterestType, BitmapImage> IMAGES = new Dictionary<PointOfInterestType, BitmapImage>() {
                { PointOfInterestType.Landmark, new BitmapImage(new Uri("pack://application:,,,/Resources/poi.png")) },
                { PointOfInterestType.Vista, new BitmapImage(new Uri("pack://application:,,,/Resources/vista.png")) },
                { PointOfInterestType.Waypoint, new BitmapImage(new Uri("pack://application:,,,/Resources/waypoint.png")) }
            };

        public PointOfInterestPushpin(PointOfInterest poi)
            : base()
        {
            if (IMAGES.ContainsKey(poi.TypeEnum))
                Image = IMAGES[poi.TypeEnum];

            if (!string.IsNullOrWhiteSpace(poi.Name))
                ToolTip = poi.Name;
        }
    }
}
