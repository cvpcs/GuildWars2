using System;
using System.Drawing;
using System.IO;

namespace GuildWars2.PvPOcr
{
    public class RedScoreBarWindow : ScoreBarWindow
    {
        public RedScoreBarWindow(bool overlayMode = true, RectangleF? position = null)
            : base("./resources/redbar_background.png",
                   "./resources/redbar_boost.png",
                   "./resources/redbar_score.png",
                   true, overlayMode, position)
        {
            Title = "Red score bar";
        }
    }
}
