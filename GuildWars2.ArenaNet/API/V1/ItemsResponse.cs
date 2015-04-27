using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.API.V1
{
    public class ItemsResponse
    {
        [JsonProperty("items")]
        public List<int> Items { get; set; }
    }
}
