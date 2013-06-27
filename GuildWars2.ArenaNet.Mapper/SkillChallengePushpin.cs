using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class SkillChallengePushpin : Pushpin
    {
        private static BitmapImage IMAGE;

        static SkillChallengePushpin()
        {
            IMAGE = new BitmapImage();
            IMAGE.BeginInit();
            IMAGE.StreamSource = Application.GetResourceStream(new Uri("/Resources/skill_point.png", UriKind.Relative)).Stream;
            IMAGE.EndInit();
        }

        public BitmapImage Image { get; private set; }

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
