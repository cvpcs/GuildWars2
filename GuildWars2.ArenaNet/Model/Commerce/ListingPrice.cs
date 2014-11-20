using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.Commerce
{
    public class ListingPrice
    {
        [JsonProperty("unit_price")]
        public int UnitPrice { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
