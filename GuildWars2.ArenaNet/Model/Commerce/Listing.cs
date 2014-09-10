using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model.Commerce;

namespace GuildWars2.ArenaNet.API.Commerce
{
    public class Listing
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("buys")]
        public List<ListingSet> Buys { get; set; }

        [JsonProperty("sells")]
        public List<ListingSet> Sells { get; set; }
    }
}
