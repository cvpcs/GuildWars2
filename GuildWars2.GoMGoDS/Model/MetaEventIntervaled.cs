using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventIntervaled : MetaEvent
    {
        private DateTime m_LastStarted = DateTime.MinValue;
        private uint m_Interval;
        private uint m_Window;
        private int m_OnTheSec;

        public MetaEventIntervaled(string id, string name, uint interval, uint window, int on_the_sec = -1)
            : base(id, name)
        {
            m_Interval = interval;
            m_Window = window;
            m_OnTheSec = on_the_sec;
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
            else if (m_LastStarted > DateTime.MinValue && prevStageId >= 0 && stageId < 0)
            {
                DateTime intervalBase = m_LastStarted;

                if (m_OnTheSec >= 0)
                    intervalBase = intervalBase.Round(new TimeSpan(0, 0, m_OnTheSec));

                uint secs_passed = Convert.ToUInt32(Math.Round((DateTime.UtcNow - intervalBase).TotalSeconds));
                MinSpawn = m_Interval - secs_passed;
                MaxSpawn = MinSpawn + m_Window;
            }

            return stageId;
        }
    }
}
