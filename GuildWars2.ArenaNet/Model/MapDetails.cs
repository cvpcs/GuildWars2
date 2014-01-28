using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class MapDetails
    {
        [JsonProperty("map_name")]
        public string MapName { get; set; }

        [JsonProperty("min_level")]
        public int MinLevel { get; set; }

        [JsonProperty("max_level")]
        public int MaxLevel { get; set; }

        [JsonProperty("default_floor")]
        public int DefaultFloor { get; set; }

        [JsonProperty("floors")]
        public List<int> Floors { get; set; }

        [JsonProperty("region_id")]
        public int RegionId { get; set; }

        [JsonProperty("region_name")]
        public string RegionName { get; set; }

        [JsonProperty("continent_id")]
        public int ContinentId { get; set; }

        [JsonProperty("continent_name")]
        public string ContinentName { get; set; }

        [JsonProperty("map_rect")]
        public List<List<double>> MapRect { get; set; }

        [JsonProperty("continent_rect")]
        public List<List<double>> ContinentRect { get; set; }
    }
}
