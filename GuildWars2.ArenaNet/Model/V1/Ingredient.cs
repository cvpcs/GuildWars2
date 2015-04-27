using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class Ingredient
    {
        [JsonProperty("item_id")]
        public int ItemId { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
