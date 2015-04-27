using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class Continent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("continent_dims")]
        public List<double> ContinentDims { get; set; }

        [JsonProperty("min_zoom")]
        public int MinZoom { get; set; }

        [JsonProperty("max_zoom")]
        public int MaxZoom { get; set; }

        [JsonProperty("floors")]
        public List<int> Floors { get; set; }
    }
}
