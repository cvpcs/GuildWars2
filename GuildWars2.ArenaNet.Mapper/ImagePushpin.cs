using System;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ImagePushpin : Pushpin
    {
        public BitmapImage Image { get; set; }
    }
}
