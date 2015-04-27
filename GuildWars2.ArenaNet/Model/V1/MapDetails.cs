using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class MapDetails : MapDetailsBase
    {
        [JsonProperty("map_name")]
        public string MapName { get; set; }

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
    }
}
