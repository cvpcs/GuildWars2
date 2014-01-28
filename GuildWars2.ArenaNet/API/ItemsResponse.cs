using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.API
{
    public class ItemsResponse
    {
        [JsonProperty("items")]
        public List<int> Items { get; set; }
    }
}
