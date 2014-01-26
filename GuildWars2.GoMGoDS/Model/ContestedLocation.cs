using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class ContestedLocation
    {
        public string Id { get { return Abbreviation.ToLower(); } }
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }

        public HashSet<Guid> CaptureEvents { get; private set; }
        public HashSet<Guid> DefendEvents { get; private set; }

        public HashSet<Guid> Events
        { get { return new HashSet<Guid>(CaptureEvents.Union(DefendEvents).Distinct()); } }

        public ContestedLocation(string name, string abbreviation)
        {
            Name = name;
            Abbreviation = abbreviation;

            CaptureEvents = new HashSet<Guid>();
            DefendEvents = new HashSet<Guid>();
        }

        public ContestedLocation AddCaptureEvent(Guid eid)
        {
            CaptureEvents.Add(eid);
            return this;
        }

        public ContestedLocation AddDefendEvent(Guid eid)
        {
            DefendEvents.Add(eid);
            return this;
        }

        public ContestedLocationStatus GetStatus(HashSet<EventState> eventStates)
        {
            HashSet<int> open = new HashSet<int>();
            HashSet<int> defend = new HashSet<int>();
            HashSet<int> capture = new HashSet<int>();

            foreach (EventState state in eventStates.Where(es => DefendEvents.Contains(es.EventId)))
            {
                switch (state.StateEnum)
                {
                    case EventStateType.Warmup:
                    case EventStateType.Preparation:
                    case EventStateType.Success:
                        open.Add(state.WorldId);
                        break;
                    case EventStateType.Active:
                        defend.Add(state.WorldId);
                        break;
                    default:
                        break;
                }
            }

            foreach (EventState state in eventStates.Where(es => CaptureEvents.Contains(es.EventId)))
            {
                if (state.StateEnum == EventStateType.Active)
                {
                    open.Remove(state.WorldId);
                    defend.Remove(state.WorldId);
                    capture.Add(state.WorldId);
                }
            }

            return new ContestedLocationStatus()
                {
                    Name = Name,
                    Abbreviation = Abbreviation,
                    OpenOn = open.ToList(),
                    DefendOn = defend.ToList(),
                    CaptureOn = capture.ToList()
                };
        }
    }
}
