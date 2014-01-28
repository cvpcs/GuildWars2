using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class ColorsResponse
    {
        [JsonProperty("colors")]
        public Dictionary<int, Color> Colors { get; set; }
    }
}
