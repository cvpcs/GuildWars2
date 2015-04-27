using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventMultiLineStage : MetaEventStage
    {
        private IDictionary<string, MetaEventStage> m_SubStages;
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

        public override HashSet<MetaEventStage.EventState> EventStates
        {
            get
            {
                return new HashSet<EventState>(m_SubStages.SelectMany(ss => ss.Value.EventStates));
            }
        }

        public MetaEventMultiLineStage(StageType type, string name, uint countdown = 0, bool isEndStage = false)
            : base(type, name, countdown, isEndStage)
        {
            m_SubStages = new Dictionary<string, MetaEventStage>();
            m_ActiveName = string.Empty;
        }

        public override MetaEventStage AddEvent(Guid ev)
        {
            return AddEvent(ev, Name);
        }

        public override MetaEventStage AddEvent(Guid ev, EventStateType state)
        {
            return AddEvent(ev, state, Name);
        }

        public MetaEventMultiLineStage AddEvent(Guid ev, string name)
        {
            return AddEvent(ev, EventStateType.Active, name)
                    .AddEvent(ev, EventStateType.Preparation, name);
        }

        public MetaEventMultiLineStage AddEvent(Guid ev, EventStateType state, string name)
        {
            if (!m_SubStages.ContainsKey(name))
                m_SubStages.Add(name, new MetaEventStage(Type, name));

            m_SubStages[name].AddEvent(ev, state);

            return this;
        }

        public MetaEventMultiLineStage AddSubstage(MetaEventStage substage)
        {
            if (!m_SubStages.ContainsKey(substage.Name))
                m_SubStages.Add(substage.Name, substage);
            else
            {
                foreach (EventState state in substage.EventStates)
                    m_SubStages[substage.Name].EventStates.Add(state);
            }

            return this;
        }
        
        public override bool IsActive(HashSet<GuildWars2.ArenaNet.Model.V1.EventState> events)
        {
            IEnumerable<EventState> eventStates = events.Select(es => new EventState() { Event = es.EventId, State = es.StateEnum }).Where(es => EventStates.Contains(es)).Distinct();
            m_ActiveName = string.Join("\n", m_SubStages.Values.Where(ss => ss.IsActive(events)).Select(ss => ss.Name).Distinct());
            return eventStates.Count() > 0;
        }
    }
}
