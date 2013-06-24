using System;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetTileSource : TileSource
    {
        public int ContinentId { get; set; }

        public int Floor { get; set; }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri("https://tiles.guildwars2.com/{c}/{f}/{z}/{x}/{y}.jpg"
                    .Replace("{c}", ContinentId.ToString())
                    .Replace("{f}", Floor.ToString())
                    .Replace("{x}", x.ToString())
                    .Replace("{y}", y.ToString())
                    .Replace("{z}", zoomLevel.ToString()));
        }
    }
}
