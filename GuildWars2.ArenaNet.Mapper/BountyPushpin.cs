using System;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class BountyPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE;

        static BountyPushpin()
        {
            IMAGE = new BitmapImage();
            IMAGE.BeginInit();
            IMAGE.StreamSource = Application.GetResourceStream(new Uri("/Resources/bounty.png", UriKind.Relative)).Stream;
            IMAGE.EndInit();
        }

        public BountyPushpin()
            : base()
        {
            Image = IMAGE;
        }
    }
}
