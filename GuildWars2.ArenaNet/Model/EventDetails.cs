using System;
using System.Collections.Generic;
using System.Linq;

namespace GuildWars2.ArenaNet.Model
{
    public class EventDetails
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public int MapId { get; set; }
        
        public List<string> Flags { get; set; }
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

        public Location Location { get; set; }
    }
}
