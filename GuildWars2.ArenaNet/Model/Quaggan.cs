using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class Quaggan
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
