using System;

namespace GuildWars2.ArenaNet
{
    public static partial class ImageServices
    {
        public static readonly string WorldMapTileServiceBaseURL = "https://tiles.guildwars2.com";

        public static readonly string RenderServiceBaseURL = "https://render.guildwars2.com";

        public enum RenderServiceFormat
        {
            PNG,
            JPG
        }
    }
}
