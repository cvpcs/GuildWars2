using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventBlockedStage : MetaEventMultiLineStage
    {
        public HashSet<EventState> BlockedEventStates { get; private set; }
        
        public MetaEventBlockedStage(string name, uint countdown = 0, bool isEndStage = false)
            : base(StageType.Blocking, name, countdown, isEndStage)
        {
            BlockedEventStates = new HashSet<EventState>();
        }

        public MetaEventBlockedStage AddBlockedEvent(Guid ev)
        {
            return AddBlockedEvent(ev, EventStateType.Preparation)
                    .AddBlockedEvent(ev, EventStateType.Active);
        }

        public MetaEventBlockedStage AddBlockedEvent(Guid ev, EventStateType state)
        {
            EventState evs = new EventState() { Event = ev, State = state };

            if (!BlockedEventStates.Contains(evs))
                BlockedEventStates.Add(evs);

            return this;
        }

        public override bool IsActive(HashSet<ArenaNet.Model.EventState> events)
        {
            return events.Where(es => BlockedEventStates.Contains(new EventState() { Event = es.EventId, State = es.StateEnum })).Count() > 0 &&
                base.IsActive(events);
        }
    }
}
