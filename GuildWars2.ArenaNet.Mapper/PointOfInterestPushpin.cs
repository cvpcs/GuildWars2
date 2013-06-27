using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class PointOfInterestPushpin : ImagePushpin
    {
        private static IDictionary<PointOfInterestType, BitmapImage> IMAGES;

        static PointOfInterestPushpin()
        {
            IMAGES = new Dictionary<PointOfInterestType, BitmapImage>();

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/poi.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES.Add(PointOfInterestType.Landmark, img);

            img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/vista.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES.Add(PointOfInterestType.Vista, img);

            img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/waypoint.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES.Add(PointOfInterestType.Waypoint, img);
        }

        public PointOfInterestPushpin(PointOfInterest poi)
            : base()
        {
            Width = 20;
            Height = 20;

            PositionOrigin = PositionOrigin.Center;

            if (IMAGES.ContainsKey(poi.TypeEnum))
                Image = IMAGES[poi.TypeEnum];

            if (!string.IsNullOrWhiteSpace(poi.Name))
                ToolTip = poi.Name;
        }
    }
}
