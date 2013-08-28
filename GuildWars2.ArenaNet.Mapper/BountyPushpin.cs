using System;
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
    public class BountyPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = new BitmapImage(new Uri("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component/Resources/bounty.png"));

        public BountyPushpin()
            : base()
        {
            Image = IMAGE;
        }
    }
}
