using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;

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
            sw.WriteLine("ev_id, spawn_time");
            foreach (EventSpawnData data in m_Data)
                sw.WriteLine("{0}, {1}", data.ev_id, data.spawn_time.ToString("G"));
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
                EventTimerDataResponse response = new EventTimerDataRequest().Execute();

                foreach (MetaEventStatus ev in response.Events)
                {
                    if (!m_EventStatus.ContainsKey(ev.Id))
                        m_EventStatus[ev.Id] = ev;
                    else
                    {
                        MetaEventStatus oldEv = m_EventStatus[ev.Id];

                        if (oldEv.StageTypeEnum == MetaEventStage.StageType.Invalid &&
                            ev.StageTypeEnum != MetaEventStage.StageType.Invalid)
                        {
                            DateTime epoch = new DateTime(1970, 1, 1);
                            DateTime oldTIme = epoch.AddMilliseconds((double)oldEv.Timestamp);
                            DateTime newTIme = epoch.AddMilliseconds((double)ev.Timestamp);

                            EventSpawnData spawnData = new EventSpawnData()
                                {
                                    ev_id = ev.Id,
                                    spawn_time = newTIme - oldTIme
                                };

                            m_Data.Add(spawnData);
                        }

                        m_EventStatus[ev.Id] = ev;
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
        }
    }
}
