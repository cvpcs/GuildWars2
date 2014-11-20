using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.Commerce
{
    public class ListingSet : ListingPrice
    {
        [JsonProperty("listings")]
        public int Listings { get; set; }
    }
}
