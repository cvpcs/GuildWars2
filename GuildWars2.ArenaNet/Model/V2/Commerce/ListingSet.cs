using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V2.Commerce
{
    public class ListingSet : ListingPrice
    {
        [JsonProperty("listings")]
        public int Listings { get; set; }
    }
}
