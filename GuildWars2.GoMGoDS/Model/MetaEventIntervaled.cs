using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventIntervaled : MetaEvent
    {
        private DateTime m_LastStarted = DateTime.MinValue;
        private uint m_Interval;
        private uint m_Window;

        public MetaEventIntervaled(string id, string name, uint interval, uint window)
            : base(id, name)
        {
            m_Interval = interval;
            m_Window = window;
        }

        public override int GetStageId(HashSet<EventState> eventStates, int prevStageId = -1)
        {
            int stageId = base.GetStageId(eventStates, prevStageId);

            if (prevStageId < 0 && stageId >= 0)
            {
                m_LastStarted = DateTime.UtcNow;
                MinSpawn = 0;
                MaxSpawn = 0;
            }
            else if (m_LastStarted > DateTime.MinValue && prevStageId >= 0 && stageId == 0)
            {
                uint secs_passed = (uint)Math.Round((DateTime.UtcNow - m_LastStarted).TotalSeconds);
                MinSpawn = m_Interval - secs_passed;
                MaxSpawn = MinSpawn + m_Window;
            }

            return stageId;
        }
    }
}
