using System;

namespace GuildWars2.ArenaNet.Model
{
    public class EventState
    {
        public int WorldId { get; set; }

        public int MapId { get; set; }

        public Guid EventId { get; set; }

        public string State { get; set; }
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
