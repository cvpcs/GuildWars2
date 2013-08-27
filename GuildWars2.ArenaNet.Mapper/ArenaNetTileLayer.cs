using System;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public class ArenaNetTileLayer : MapTileLayer
    {
        public ArenaNetTileLayer()
            : base()
        {
#if SILVERLIGHT
            TileSources.Add(new ArenaNetTileLayerSource());
#else
            TileSource = new ArenaNetTileLayerSource();
#endif
        }
    }
}
