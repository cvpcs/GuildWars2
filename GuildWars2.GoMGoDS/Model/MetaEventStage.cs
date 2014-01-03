using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventStage
    {
        public virtual StageType Type { get; private set; }
        public virtual string Name { get; private set; }
        public bool IsEndStage { get; private set; }

        public IList<EventState> EventStates { get; private set; }

        // 0 = no countdown
        // MaxValue = continue from previous
        // other = value in seconds
        public uint Countdown { get; private set; }

        public MetaEventStage(StageType type, string name, uint countdown = 0, bool isEndStage = false)
        {
            Type = type;
            Name = name;
            EventStates = new List<EventState>();

            Countdown = countdown;

            IsEndStage = isEndStage;
        }

        public virtual MetaEventStage AddEvent(Guid ev)
        {
            return AddEvent(ev, EventStateType.Preparation)
                    .AddEvent(ev, EventStateType.Active);
        }

        public virtual MetaEventStage AddEvent(Guid ev, EventStateType state)
        {
            EventState evs = new EventState() { Event = ev, State = state };

            if (!EventStates.Contains(evs))
                EventStates.Add(evs);

            return this;
        }

        public virtual bool IsActive(IList<GuildWars2.ArenaNet.Model.EventState> events)
        {
            return events.Where(es => EventStates.Contains(new EventState() { Event = es.EventId, State = es.StateEnum })).Count() > 0;
        }

        public virtual bool IsSuccessful(IList<GuildWars2.ArenaNet.Model.EventState> events)
        {
            IEnumerable<Guid> eventIds = EventStates.Select(es => es.Event).Distinct();

            return events.Where(es => eventIds.Contains(es.EventId) && es.StateEnum == EventStateType.Success).Count() == eventIds.Count();
        }

        public virtual bool IsFailed(IList<GuildWars2.ArenaNet.Model.EventState> events)
        {
            IEnumerable<Guid> eventIds = EventStates.Select(es => es.Event).Distinct();

            return events.Where(es => eventIds.Contains(es.EventId) && es.StateEnum == EventStateType.Fail).Count() > 0;
        }

        public struct EventState
        {
            public Guid Event;
            public EventStateType State;
        }

        public enum StageType
        {
            Reset,
            Invalid,
            Recovery,
            Blocking,
            PreEvent,
            Boss
        }
    }
}
