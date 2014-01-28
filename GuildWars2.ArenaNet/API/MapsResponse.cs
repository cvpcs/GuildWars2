using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class MapsResponse
    {
        [JsonProperty("maps")]
        public Dictionary<int, MapDetails> Maps { get; set; }
    }
}
