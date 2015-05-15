using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.API.V1
{
    public class SkinsResponse
    {
        [JsonProperty("skins")]
        public List<int> Skins { get; set; }
    }
}
