using System;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class SkillChallengePushpin : ImagePushpin
    {
        private static BitmapImage IMAGE;

        static SkillChallengePushpin()
        {
            IMAGE = new BitmapImage();
            IMAGE.BeginInit();
            IMAGE.StreamSource = Application.GetResourceStream(new Uri("/Resources/skill_point.png", UriKind.Relative)).Stream;
            IMAGE.EndInit();
        }

        public SkillChallengePushpin(MappedModel skill_challenge)
            : base()
        {
            Width = 20;
            Height = 20;

            PositionOrigin = PositionOrigin.Center;

            Image = IMAGE;
        }
    }
}
