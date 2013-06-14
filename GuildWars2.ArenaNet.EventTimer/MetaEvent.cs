using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.EventTimer
{
    public class MetaEvent
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public uint MinSpawn { get; private set; }
        public uint MaxSpawn { get; private set; }

        public IList<MetaEventStage> Stages { get; private set; }

        public MetaEvent(string id, string name)
        {
            Id = id;
            Name = name;

            MinSpawn = 0;
            MaxSpawn = 0;

            Stages = new List<MetaEventStage>();
        }

        public MetaEvent(string id, string name, uint min, uint max)
            : this(id, name)
        {
            MinSpawn = min;
            MaxSpawn = max;
        }

        public MetaEvent AddStage(MetaEventStage stage)
        {
            Stages.Add(stage);
            return this;
        }

        public int GetStageId(IList<EventState> eventStates)
        {
            int stageId = -1;

            for (int i = 0; i < Stages.Count; i++)
            {
                MetaEventStage stage = Stages[i];

                IEnumerable<EventState> stageEvents = eventStates.Where(es => stage.EventStates.Contains(new MetaEventStage.EventState() { Event = es.EventId, State = es.StateEnum }));

                if (stageEvents.Where(es => es.StateEnum == EventStateType.Active).Count() > 0)
                {
                    stageId = i;
                    break;
                }
                else if (stageEvents.Where(es => es.StateEnum == EventStateType.Preparation).Count() > 0)
                {
                    stageId = i;
                    break;
                }
                else if (stageEvents.Where(es => es.StateEnum == EventStateType.Warmup).Count() > 0)
                {
                    if (stage.Type != MetaEventStage.StageType.Recovery ||
                            (i > 1 && Stages[i - 1].Type == MetaEventStage.StageType.Recovery
                                    && eventStates.Where(es => es.StateEnum == EventStateType.Success && Stages[i - 1].EventStates.Select(ses => ses.Event).Contains(es.EventId)).Count() > 0))
                    {
                        stageId = i;
                        break;
                    }
                }
            }

            return stageId;
        }
    }
}
