using System;
using System.Windows;
using System.Windows.Media.Imaging;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class PlayerPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = ResourceUtility.LoadBitmapImage("/Resources/player_position.png");

        public PlayerPushpin()
            : base()
        {
            Image = IMAGE;

            ImageWidth = 32;
            ImageHeight = 32;
        }
    }
}
