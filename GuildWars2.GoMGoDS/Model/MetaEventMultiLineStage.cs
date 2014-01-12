using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventMultiLineStage : MetaEventStage
    {
        public override StageType Type
        {
            get
            {
                return base.Type;
            }
        }

        private IDictionary<EventState, string> m_EventStateNames;
        private string m_ActiveName;
        public override string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(m_ActiveName))
                    return m_ActiveName;
                else
                    return base.Name;
            }
        }

        public MetaEventMultiLineStage(StageType type, string name, uint countdown = 0, bool isEndStage = false)
            : base(type, name, countdown, isEndStage)
        {
            m_EventStateNames = new Dictionary<EventState, string>();
            m_ActiveName = string.Empty;
        }

        public MetaEventMultiLineStage AddEvent(Guid ev, string name)
        {
            return AddEvent(ev, EventStateType.Active, name)
                    .AddEvent(ev, EventStateType.Preparation, name);
        }

        public MetaEventMultiLineStage AddEvent(Guid ev, EventStateType state, string name)
        {
            AddEvent(ev, state);
            m_EventStateNames[new EventState() { Event = ev, State = state }] = name;

            return this;
        }

        public MetaEventMultiLineStage AddSubstage(MetaEventStage substage)
        {
            foreach (EventState ev in substage.EventStates)
            {
                AddEvent(ev.Event, ev.State);
                m_EventStateNames[ev] = substage.Name;
            }

            return this;
        }
        
        public override bool IsActive(HashSet<GuildWars2.ArenaNet.Model.EventState> events)
        {
            IEnumerable<EventState> eventStates = events.Select(es => new EventState() { Event = es.EventId, State = es.StateEnum }).Where(es => EventStates.Contains(es)).Distinct();
            m_ActiveName = string.Join("\n", m_EventStateNames.Where(kvp => eventStates.Contains(kvp.Key)).Select(kvp => kvp.Value).Distinct());
            return eventStates.Count() > 0;
        }
    }
}
