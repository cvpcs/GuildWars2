using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model.Commerce;

namespace GuildWars2.ArenaNet.API.Commerce
{
    public class ExchangeResponse
    {
        [JsonProperty("coins_per_gem")]
        public int CoinsPerGem { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
