using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class EventsResponse
    {
        [JsonProperty("events")]
        public List<EventState> Events { get; set; }
    }
}
