using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.API.V1
{
    public class BuildResponse
    {
        [JsonProperty("build_id")]
        public int BuildId { get; set; }
    }
}
