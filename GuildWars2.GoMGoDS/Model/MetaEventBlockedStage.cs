using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventBlockedStage : MetaEventMultiLineStage
    {
        private HashSet<EventState> m_BlockedEventStates;

        public override HashSet<EventState> EventStates
        {
            get
            {
                return new HashSet<EventState>(base.EventStates.Union(m_BlockedEventStates));
            }
        }
        
        public MetaEventBlockedStage(string name, uint countdown = 0, bool isEndStage = false)
            : base(StageType.Blocking, name, countdown, isEndStage)
        {
            m_BlockedEventStates = new HashSet<EventState>();
        }

        public MetaEventBlockedStage AddBlockedEvent(Guid ev)
        {
            return AddBlockedEvent(ev, EventStateType.Preparation)
                    .AddBlockedEvent(ev, EventStateType.Active);
        }

        public MetaEventBlockedStage AddBlockedEvent(Guid ev, EventStateType state)
        {
            m_BlockedEventStates.Add(new EventState() { Event = ev, State = state });
            return this;
        }

        public override bool IsActive(HashSet<ArenaNet.Model.EventState> events)
        {
            return events.Where(es => m_BlockedEventStates.Contains(new EventState() { Event = es.EventId, State = es.StateEnum })).Count() > 0 &&
                base.IsActive(events);
        }

        public override bool IsSuccessful(HashSet<ArenaNet.Model.EventState> events)
        {
            return events.Where(es => m_BlockedEventStates.Contains(new EventState() { Event = es.EventId, State = es.StateEnum })).Count() > 0 && 
                base.IsSuccessful(events);
        }

        public override bool IsFailed(HashSet<ArenaNet.Model.EventState> events)
        {
            return events.Where(es => m_BlockedEventStates.Contains(new EventState() { Event = es.EventId, State = es.StateEnum })).Count() > 0 && 
                base.IsFailed(events);
        }
    }
}
