using System;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetTileLayer : MapTileLayer
    {
        public ArenaNetTileLayer()
        {
            TileSource = new ArenaNetTileSource()
                {
                    ContinentId = 1,
                    Floor = 1
                };
        }

    }
}
