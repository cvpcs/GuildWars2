using System;
using System.Windows;
using System.Windows.Media.Imaging;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class SkillChallengePushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = LoadImageResource("/Resources/skill_point.png");

        public SkillChallengePushpin(MappedModel skill_challenge)
            : base()
        {
            Image = IMAGE;
        }
    }
}
