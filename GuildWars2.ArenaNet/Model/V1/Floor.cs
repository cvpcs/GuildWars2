using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class Floor
    {
        [JsonProperty("texture_dims")]
        public List<int> TextureDims { get; set; }

        [JsonProperty("regions")]
        public Dictionary<string, FloorRegion> Regions { get; set; }
    }
}
