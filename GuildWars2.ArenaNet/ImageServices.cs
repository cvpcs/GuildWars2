using System;

namespace GuildWars2.ArenaNet
{
    public static partial class ImageServices
    {
        private static Uri m_WorldMapTileServiceBaseUri = new Uri("https://tiles.guildwars2.com");

        private static Uri m_RenderServiceBaseURI = new Uri("https://render.guildwars2.com");

        public enum RenderServiceFormat
        {
            PNG,
            JPG
        }
    }
}
