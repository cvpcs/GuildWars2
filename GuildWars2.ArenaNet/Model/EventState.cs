using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class EventState
    {
        [JsonProperty("world_id")]
        public int WorldId { get; set; }

        [JsonProperty("map_id")]
        public int MapId { get; set; }

        [JsonProperty("event_id")]
        public Guid EventId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
        [JsonIgnore]
        public EventStateType StateEnum
        {
            get
            {
                EventStateType type;
                if (Enum.TryParse<EventStateType>(State, true, out type))
                    return type;

                return EventStateType.Invalid;
            }
        }
    }
}
