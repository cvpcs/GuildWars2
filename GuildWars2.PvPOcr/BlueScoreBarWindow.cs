using System;
using System.Drawing;
using System.IO;

namespace GuildWars2.PvPOcr
{
    public class BlueScoreBarWindow : ScoreBarWindow
    {
        public BlueScoreBarWindow(bool overlayMode = true, RectangleF? position = null)
            : base("./resources/bluebar_background.png",
                   "./resources/bluebar_boost.png",
                   "./resources/bluebar_score.png",
                   false, overlayMode, position)
        {
            Title = "Blue score bar";
        }
    }
}
