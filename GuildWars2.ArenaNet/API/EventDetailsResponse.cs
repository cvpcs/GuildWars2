using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class EventDetailsResponse
    {
        [JsonProperty("events")]
        public Dictionary<Guid, EventDetails> Events { get; set; }
    }
}
