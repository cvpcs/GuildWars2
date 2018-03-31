using System;
using System.Drawing;
using System.IO;

namespace GuildWars2.PvPOcr
{
    public class RedScoreBarWindow : ScoreBarWindow
    {
        public RedScoreBarWindow(bool overlayMode = true, RectangleF? position = null)
            : base(new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/redbar_background.png")),
                   new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/redbar_boost.png")),
                   new Uri(Path.Combine(Environment.CurrentDirectory, "./resources/redbar_score.png")),
                   true, overlayMode, position)
        {
            Title = "Red score bar";
        }
    }
}
