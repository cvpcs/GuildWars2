using System;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetTileLayer : MapTileLayer
    {
        public ArenaNetTileLayer()
            : base()
        {
            TileSource = new ArenaNetTileLayerSource();
        }
    }
}
