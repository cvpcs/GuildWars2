using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class Color
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("base_rgb")]
        public List<int> BaseRgb { get; set; }

        [JsonProperty("cloth")]
        public MaterialColor Cloth { get; set; }

        [JsonProperty("leather")]
        public MaterialColor Leather { get; set; }

        [JsonProperty("metal")]
        public MaterialColor Metal { get; set; }
    }
}
