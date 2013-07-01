using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.SyntaxError.Model
{
    public class MetaEventStage
    {
        public StageType Type { get; private set; }
        public string Name { get; private set; }
        public IList<EventState> EventStates { get; private set; }

        // 0 = no countdown
        // MaxValue = continue from previous
        // other = value in seconds
        public uint Countdown { get; private set; }

        public MetaEventStage(StageType type, string desc, uint countdown = 0)
        {
            Type = type;
            Name = desc;
            EventStates = new List<EventState>();

            Countdown = countdown;
        }

        public MetaEventStage AddEvent(Guid ev)
        {
            return AddEvent(ev, EventStateType.Preparation)
                    .AddEvent(ev, EventStateType.Active);
        }

        public MetaEventStage AddEvent(Guid ev, EventStateType state)
        {
            EventStates.Add(new EventState() { Event = ev, State = state });
            return this;
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
            Window,
            Recovery,
            Blocking,
            PreEvent,
            Boss
        }
    }
}
