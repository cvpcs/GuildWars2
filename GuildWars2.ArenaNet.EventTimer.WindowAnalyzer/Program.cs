using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.API;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.EventTimer.WindowAnalyzer
{
    public class Program
    {
        private static TimeSpan m_PollRate = new TimeSpan(0, 0, 30);
        private static Timer m_Timer;
        private static int m_TimerSync;

        private static IDictionary<string, MetaEventStatus> m_EventStatus;
        private static IList<EventSpawnData> m_Data;

        public static void Main(string[] args)
        {
            Console.Write("Loading current event status . . . ");
            m_EventStatus = new EventTimerDataRequest().Execute().Events.ToDictionary(ev => ev.Id);
            m_Data = new List<EventSpawnData>();
            Console.WriteLine("Done.");

            // start the timer
            m_TimerSync = 0;
            m_Timer = new Timer(m_PollRate.TotalMilliseconds);
            m_Timer.Elapsed += WorkerThread;
            m_Timer.Start();

            Console.WriteLine("Beginning window analysis. Press any key to cease gathering data.");
            Console.ReadKey();
            Console.WriteLine();

            Console.Write("Halting window analysis . . . ");

            // stop timer
            m_Timer.Stop();

            // wait for any existing threads to complete
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref m_TimerSync, -1, 0) == 0);

            Console.WriteLine("Done.");

            Console.Write("Writing data to file [data.csv] . . . ");
            StreamWriter sw = new StreamWriter("data.csv");
            sw.WriteLine("ev_id, spawn_time, failed");
            foreach (EventSpawnData data in m_Data)
                sw.WriteLine("{0}, {1}, {2}", data.ev_id, data.spawn_time.ToString("G"), data.failed);
            sw.Close();
        }

        public static void WorkerThread(object sender, ElapsedEventArgs e)
        {
            // attempt to set the sync, if another of us is running, just exit
            if (Interlocked.CompareExchange(ref m_TimerSync, 1, 0) != 0)
                return;

            // wrap in a try-catch so we can release our interlock if something fails
            try
            {
                EventsResponse response = new EventsRequest(1007).Execute();

                long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                IList<EventState> metaEvents = response.Events.Where(es => MetaEventDefinitions.EventList.Contains(es.EventId)).ToList();

                foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                {
                    if (m_EventStatus.ContainsKey(meta.Id))
                    {
                        int stageId = meta.GetStageId(metaEvents, m_EventStatus[meta.Id].StageId);

                        MetaEventStatus oldevs = m_EventStatus[meta.Id];
                        MetaEventStatus newevs = new MetaEventStatus()
                            {
                                Id = meta.Id,
                                StageId = stageId,
                                StageTypeEnum = (stageId >= 0 ? meta.Stages[stageId].Type : MetaEventStage.StageType.Invalid),
                                Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds
                            };

                        if (stageId < 0 && oldevs.StageId >= 0)
                            newevs.StageName = meta.Stages[oldevs.StageId].IsFailed(metaEvents).ToString();

                        if (oldevs.StageTypeEnum == MetaEventStage.StageType.Invalid &&
                            newevs.StageTypeEnum != MetaEventStage.StageType.Invalid)
                        {
                            DateTime epoch = new DateTime(1970, 1, 1);
                            DateTime oldTIme = epoch.AddMilliseconds((double)oldevs.Timestamp);
                            DateTime newTIme = epoch.AddMilliseconds((double)newevs.Timestamp);

                            EventSpawnData spawnData = new EventSpawnData()
                                {
                                    ev_id = meta.Id,
                                    spawn_time = newTIme - oldTIme,
                                    failed = oldevs.StageName
                                };

                            m_Data.Add(spawnData);
                        }

                        if (oldevs.StageId != newevs.StageId)
                        {
                            m_EventStatus[meta.Id] = newevs;
                        }
                    }
                }
            }
            catch (Exception)
            { }

            // reset sync to 0
            Interlocked.Exchange(ref m_TimerSync, 0);
        }

        public struct EventSpawnData
        {
            public string ev_id;
            public TimeSpan spawn_time;
            public string failed;
        }
    }
}
