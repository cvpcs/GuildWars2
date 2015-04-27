using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V2
{
    public class Quaggan
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
