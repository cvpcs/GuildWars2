using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class FloorRegion
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label_coord")]
        public List<double> LabelCoord { get; set; }

        [JsonProperty("maps")]
        public Dictionary<int, FloorMapDetails> Maps { get; set; }
    }
}
