using System.Drawing;

namespace GuildWars2.PvPOcr
{
    public class AppConfig
    {
        public bool UseOverlayMode = true;
        public bool UseLiveSetup = true;
        public Rectangle RedSection = new Rectangle(0, 0, 1, 1);
        public Rectangle BlueSection = new Rectangle(0, 0, 1, 1);
        public RectangleF RedScoreBarPosition = new RectangleF(-1, -1, -1, -1);
        public RectangleF BlueScoreBarPosition = new RectangleF(-1, -1, -1, -1);
    }
}
