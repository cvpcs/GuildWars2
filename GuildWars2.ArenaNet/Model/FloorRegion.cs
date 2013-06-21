using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class FloorRegion
    {
        public string Name { get; set; }

        public List<int> LabelCoord { get; set; }

        public Dictionary<string, FloorMapDetails> Maps { get; set; }
    }
}
