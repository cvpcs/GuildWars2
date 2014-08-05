using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using GuildWars2.GoMGoDS.API;
using GuildWars2.GoMGoDS.Model;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class EventScheduleAPI : APIBase<EventScheduleResponse>
    {
        #region APIBase
        public override string RequestPath { get { return "/eventschedule.json"; } }

        public override void Init(IDbConnection dbConn) { }

        protected override EventScheduleResponse GetData(IDictionary<string, string> _get)
        {
            DateTime timestamp = DateTime.UtcNow;
            uint slot_secs_remaining = 900 - ((uint)Math.Floor(timestamp.TimeOfDay.TotalSeconds) % 900);

            int slot = MetaEventSchedule.GetSlot(timestamp);

            EventScheduleResponse data = new EventScheduleResponse()
                {
                    Timestamp = (long)(timestamp - new DateTime(1970, 1, 1)).TotalMilliseconds,
                    Events = new List<MetaEventStatus>()
                };

            SortedDictionary<int, string> meta_slot_order = new SortedDictionary<int, string>();

            foreach (string meid in MetaEventSchedule.MetaEventList)
            {
                if (meid == null)
                    continue;

                int meta_slot = -1;

                for (int i = slot; meta_slot < 0; i++)
                {
                    if (MetaEventSchedule.MetaEventRotation[i % MetaEventSchedule.MetaEventRotation.Length] == meid)
                        meta_slot = i;
                }

                meta_slot_order.Add(meta_slot, meid);
            }

            foreach (KeyValuePair<int, string> entry in meta_slot_order)
            {
                int meta_slot = entry.Key;

                MetaEvent meta = MetaEventDefinitions.MetaEvents.Where(me => me.Id == entry.Value).First();

                MetaEventStatus status = new MetaEventStatus()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        MinCountdown = 0,
                        MaxCountdown = 0,
                        StageId = -1,
                        StageType = null,
                        StageName = null,
                        Timestamp = data.Timestamp
                    };

                if (meta_slot == slot)
                {
                    status.StageId = 2;
                    status.StageTypeEnum = MetaEventStage.StageType.Boss;
                    status.StageName = "This boss is currently active.";
                    status.Countdown = slot_secs_remaining;
                }
                else if (meta_slot == slot + 1)
                {
                    status.StageId = 1;
                    status.StageTypeEnum = MetaEventStage.StageType.PreEvent;
                    status.StageName = "This boss is on deck.";
                    status.Countdown = slot_secs_remaining;
                }
                else
                {
                    status.StageId = 0;
                    status.StageTypeEnum = MetaEventStage.StageType.Invalid;
                    status.StageName = "This boss is on cooldown.";
                    status.Countdown = ((uint)(meta_slot - (slot + 1)) * 900) + slot_secs_remaining;
                }

                data.Events.Add(status);
            }

            return data;
        }
        #endregion
    }
}
