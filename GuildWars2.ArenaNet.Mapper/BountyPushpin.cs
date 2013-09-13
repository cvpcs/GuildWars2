using System;
using System.Windows;
using System.Windows.Media.Imaging;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class BountyPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = ResourceUtility.LoadBitmapImage("/Resources/bounty.png");

        public BountyPushpin()
            : base()
        {
            Image = IMAGE;
        }
    }
}
