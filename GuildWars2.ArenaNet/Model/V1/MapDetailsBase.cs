using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public abstract class MapDetailsBase
    {
        [JsonProperty("min_level")]
        public int MinLevel { get; set; }

        [JsonProperty("max_level")]
        public int MaxLevel { get; set; }

        [JsonProperty("default_floor")]
        public int DefaultFloor { get; set; }

        [JsonProperty("map_rect")]
        public List<List<double>> MapRect { get; set; }

        [JsonProperty("continent_rect")]
        public List<List<double>> ContinentRect { get; set; }
    }
}
