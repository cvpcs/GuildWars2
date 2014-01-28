using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class ContinentsResponse
    {
        [JsonProperty("continents")]
        public Dictionary<int, Continent> Continents { get; set; }
    }
}
