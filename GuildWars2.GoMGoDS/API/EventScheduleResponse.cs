using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    public class EventScheduleResponse
    {
        [JsonProperty("timestamp")]
        public long Timestamp;

        [JsonProperty("events")]
        public List<MetaEventStatus> Events;
    }
}
