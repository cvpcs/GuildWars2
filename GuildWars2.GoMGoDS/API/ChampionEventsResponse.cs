using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.GoMGoDS.API
{
    public class ChampionEventsResponse
    {
        [JsonProperty("champion_events")]
        public List<Guid> ChampionEvents;
    }
}
