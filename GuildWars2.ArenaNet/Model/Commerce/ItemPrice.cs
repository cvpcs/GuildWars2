using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.Commerce
{
    public class ItemPrice
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("buys")]
        public ListingPrice Buys { get; set; }

        [JsonProperty("sells")]
        public ListingPrice Sells { get; set; }
    }
}
