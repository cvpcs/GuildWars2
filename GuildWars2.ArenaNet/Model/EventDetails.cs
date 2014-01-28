using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class EventDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("map_id")]
        public int MapId { get; set; }

        [JsonProperty("flags")]
        public List<string> Flags { get; set; }
        [JsonIgnore]
        public EventFlagType FlagsEnum
        {
            get
            {
                EventFlagType typeAll = EventFlagType.None;

                foreach (string item in Flags)
                {
                    EventFlagType type;
                    if (Enum.TryParse<EventFlagType>(item.Replace("_", string.Empty), true, out type))
                        typeAll |= type;
                    else
                        typeAll |= EventFlagType.Invalid;
                }

                return typeAll;
            }
        }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }
}
