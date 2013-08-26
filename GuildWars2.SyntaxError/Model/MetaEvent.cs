using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.SyntaxError.Model
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

        public int GetStageId(IList<EventState> eventStates, int prevStageId = -1)
        {
            int stageId = -1;

            for (int i = 0; i < Stages.Count; i++)
            {
                if (Stages[i].IsActive(eventStates))
                {
                    stageId = i;
                    break;
                }
            }

            /* The following logic checks if the previous stage succeeded and therefore the stage is active.
             * This is to deal with meta events that don't immediately put the next stage events into Perparation
             * or Active status. This will only execute if we have yet to find a stage, the previous stage is
             * supplied, and the previous stage is not the last stage. If all of these conditions are met, set the
             * stage to the prevStageId + 1 if all of the previous stage's events are at status Success.
             */
            if (stageId < 0 && prevStageId >= 0 && prevStageId < Stages.Count - 1)
            {
                MetaEventStage prevStage = Stages[prevStageId];

                if (!prevStage.IsEndStage && prevStage.IsSuccessful(eventStates))
                    stageId = prevStageId;
            }

            return stageId;
        }
    }
}
