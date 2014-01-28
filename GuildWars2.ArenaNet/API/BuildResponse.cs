using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.API
{
    public class BuildResponse
    {
        [JsonProperty("build_id")]
        public int BuildId { get; set; }
    }
}
