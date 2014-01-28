using System;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetTileLayerSource : TileSource
    {
        public int ContinentId { get; set; }
        public int Floor { get; set; }

        public ArenaNetTileLayerSource(int continent_id = 1, int floor = 1)
        {
            ContinentId = continent_id;
            Floor = floor;
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri(string.Format("{0}/{1}/{2}/{5}/{3}/{4}.jpg",
                    ImageServices.WorldMapTileServiceBaseURL,
                    ContinentId,
                    Floor,
                    x,
                    y,
                    zoomLevel));
        }
    }
}
