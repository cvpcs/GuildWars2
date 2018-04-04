using System.Drawing;
using LibHotKeys;

namespace GuildWars2.PvPOcr
{
    public class AppConfig
    {
        public bool UseOverlayMode;
        public bool UseLiveSetup;
        public Rectangle RedSection;
        public Rectangle BlueSection;
        public RectangleF RedScoreBarPosition;
        public RectangleF BlueScoreBarPosition;
        public ImageModulationParameters RedScoreBarModulation;
        public ImageModulationParameters BlueScoreBarModulation;
        public HotKey ScreenshotHotKey;
    }
}
