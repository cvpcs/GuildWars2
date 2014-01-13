using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEvent
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public uint MinSpawn { get; private set; }
        public uint MaxSpawn { get; private set; }

        public HashSet<MetaEventStage> Stages { get; private set; }

        public MetaEvent(string id, string name)
        {
            Id = id;
            Name = name;

            MinSpawn = 0;
            MaxSpawn = 0;

            Stages = new HashSet<MetaEventStage>();
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

        public int GetStageId(HashSet<EventState> eventStates, int prevStageId = -1)
        {
            int stageId = -1;
            int blockedStageId = -1;

            IEnumerator<MetaEventStage> itr = Stages.GetEnumerator();
            for (int i = 0; itr.MoveNext(); i++)
            {
                if (itr.Current.IsActive(eventStates))
                {
                    if (itr.Current.Type == MetaEventStage.StageType.Blocking)
                        blockedStageId = i;
                    else
                    {
                        stageId = i;
                        break;
                    }
                }
            }

            /* Recovery states need special treatment.
             * If our current stage is recovery and consists entirely of warmups, we need to make sure that the
             * previous stage was a recovery and was successful to ensure that we were actually in recovery.
             */
            if (stageId >= 0 && itr.Current.Type == MetaEventStage.StageType.Recovery &&
                itr.Current.EventStates.Where(es => es.State != EventStateType.Warmup).Count() == 0)
            {
                if (!(prevStageId >= 0 && itr.Current.Type == MetaEventStage.StageType.Recovery &&
                    itr.Current.IsSuccessful(eventStates)))
                    stageId = -1;
            }

            /* The following logic checks if the previous stage succeeded and therefore the stage is active.
             * This is to deal with meta events that don't immediately put the next stage events into Perparation
             * or Active status. This will only execute if we have yet to find a stage, the previous stage is
             * supplied, and the previous stage is not the last stage. If all of these conditions are met, set the
             * stage to the prevStageId + 1 if all of the previous stage's events are at status Success.
             */
            if (stageId < 0 && prevStageId >= 0 && prevStageId < Stages.Count - 1)
            {
                MetaEventStage prevStage = Stages.ElementAt(prevStageId);

                if (!prevStage.IsEndStage && prevStage.IsSuccessful(eventStates))
                    stageId = prevStageId;
            }

            /* Lastly, if we still don't have a stage id, but we have a blocked stage id, then we use the blocked stage.
             * We do this because blocked events aren't really part of the meta, so we don't want them showing up most of
             * the time. Thus we always keep the non-blocked stage if it exists.
             */
            if (stageId < 0 && blockedStageId > 0)
                stageId = blockedStageId;

            return stageId;
        }
    }
}
