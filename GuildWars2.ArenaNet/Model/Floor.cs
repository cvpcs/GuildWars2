using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class Floor
    {
        public List<int> TextureDims { get; set; }

        public Dictionary<string, FloorRegion> Regions { get; set; }
    }
}
