using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.EventTimer
{
    [DataContract]
    public class MetaEventStage
    {
        [DataMember(Name = "type")]
        private string m_Type;
        public StageType Type
        {
            get { return (StageType)Enum.Parse(typeof(StageType), m_Type, true); }
            private set { m_Type = Enum.GetName(typeof(StageType), value).ToLower(); }
        }

        [DataMember(Name = "name")]
        public string Name { get; private set; }
        [DataMember(Name = "event_states")]
        public IList<EventState> EventStates { get; private set; }

        // 0 = no countdown
        // MaxValue = continue from previous
        // other = value in seconds
        [DataMember(Name = "countdown")]
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

        [DataContract]
        public class EventState
        {
            [DataMember(Name = "event")]
            private string m_Event;
            public Guid Event
            {
                get { return new Guid(m_Event); }
                set { m_Event = value.ToString(); }
            }

            [DataMember(Name = "state")]
            private string m_State;
            public EventStateType State
            {
                get { return (EventStateType)Enum.Parse(typeof(EventStateType), m_State, true); }
                set { m_State = Enum.GetName(typeof(EventStateType), value).ToLower(); }
            }
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
