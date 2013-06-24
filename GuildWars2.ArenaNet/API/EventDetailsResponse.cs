using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class EventDetailsResponse
    {
        public Dictionary<string, EventDetails> Events { get; set; }
    }
}
