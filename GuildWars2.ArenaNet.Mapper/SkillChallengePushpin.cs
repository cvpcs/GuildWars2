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
    public class SkillChallengePushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = new BitmapImage(new Uri("pack://application:,,,/Resources/skill_point.png"));

        public SkillChallengePushpin(MappedModel skill_challenge)
            : base()
        {
            Image = IMAGE;
        }
    }
}
