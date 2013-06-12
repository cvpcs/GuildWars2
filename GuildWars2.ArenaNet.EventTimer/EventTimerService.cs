using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using TimeoutException = System.TimeoutException;
using Timer = System.Timers.Timer;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.EventTimer
{
    public partial class EventTimerService : ServiceBase
    {
        private static TimeSpan p_PollRate = new TimeSpan(0, 0, 30);
        private static DataContractJsonSerializer p_Serializer = new DataContractJsonSerializer(typeof(EventTimerData));

        private Timer m_Timer;
        private int m_TimerSync;

        private FileInfo m_LogFile = new FileInfo(ConfigurationManager.AppSettings["log_file"]);

        private FileInfo m_JsonFile = new FileInfo(ConfigurationManager.AppSettings["json_file"]);
        private HttpJsonServer m_JsonServer = new HttpJsonServer(uint.Parse(ConfigurationManager.AppSettings["json_server_port"]));

        private EventTimerData m_TimerData = null;
        private SpinLock m_TimerDataLock = new SpinLock();
        private IDictionary<string, int> m_StatusListMap = null;

        public EventTimerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogLine("Starting service...");

            // load up our database
            try
            {
                using (FileStream stream = m_JsonFile.Open(FileMode.Open, FileAccess.Read))
                {
                    LogLine("Loading database from file...");

                    m_TimerData = p_Serializer.ReadObject(stream) as EventTimerData;
                    m_StatusListMap = new Dictionary<string, int>();
                    for (int i = 0; i < m_TimerData.Events.Count; i++)
                    {
                        MetaEventStatus status = m_TimerData.Events[i];
                        m_StatusListMap[status.Id] = i;
                    }

                    LogLine("Database loading complete");
                }
            }
            catch { }

            BuildResponse build = new BuildRequest().Execute();
            int buildId = (build != null ? build.BuildId : -1);
            if (m_TimerData == null || m_TimerData.Build != buildId)
            {
                m_TimerData = new EventTimerData();
                ResetTimers(buildId);
            }

            // start the http json server
            m_JsonServer.OnRequestReceived += GetJson;
            m_JsonServer.Start();

            // start the timer
            m_TimerSync = 0;
            m_Timer = new Timer(p_PollRate.TotalMilliseconds);
            m_Timer.Elapsed += WorkerThread;
            m_Timer.Start();

            LogLine("Service started");
        }

        protected override void OnStop()
        {
            LogLine("Stopping service...");

            // shut down our json http server
            m_JsonServer.Stop();

            // stop timer
            m_Timer.Stop();

            // wait for any existing threads to complete
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref m_TimerSync, -1, 0) == 0);

            // zero out our maps
            m_StatusListMap = null;
            m_TimerData = null;

            LogLine("Service stopped");
            LogLine();
        }

        private string GetJson()
        {
            bool lockTaken = false;
            string data = string.Empty;
            try
            {
                m_TimerDataLock.Enter(ref lockTaken);

                MemoryStream stream = new MemoryStream();
                p_Serializer.WriteObject(stream, m_TimerData);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                data = reader.ReadToEnd();
                reader.Close();
            }
            finally
            {
                if (lockTaken)
                    m_TimerDataLock.Exit();
            }

            return data;
        }

        private void WorkerThread(object sender, ElapsedEventArgs e)
        {
            // attempt to set the sync, if another of us is running, just exit
            if (Interlocked.CompareExchange(ref m_TimerSync, 1, 0) != 0)
                return;

            // wrap in a try-catch so we can release our interlock if something fails
            try
            {
                LogLine("Worker thread executing...");

                // check if build has changed, reset timers if it has
                BuildResponse build = new BuildRequest().Execute();
                if (build != null && build.BuildId != m_TimerData.Build)
                    ResetTimers(build.BuildId);

                // get data
                EventsResponse response = new EventsRequest(1007).Execute();
                if (response == null)
                    throw new TimeoutException("Request timed out when attempting to retrieve event data.");

                long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                IList<EventState> metaEvents = response.Events.Where(es => MetaEventDefinitions.EventList.Contains(es.EventId)).ToList();

                IDictionary<int, MetaEventStatus> changedStatuses = new Dictionary<int, MetaEventStatus>();

                // discover meta-event states
                foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                {
                    MetaEvent.MetaEventState state = meta.GetState(metaEvents);

                    // stock state
                    MetaEventStatus status = new MetaEventStatus()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        MinCountdown = meta.MinSpawn,
                        MaxCountdown = meta.MaxSpawn,
                        StageId = state.StageId,
                        StageType = null,
                        StageName = null,
                        Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds
                    };

                    // we're in a stage
                    if (state.StageId >= 0)
                    {
                        MetaEventStage stage = meta.Stages[state.StageId];

                        if (stage.Countdown > 0 && stage.Countdown != uint.MaxValue)
                        {
                            status.Countdown = stage.Countdown;
                        }
                        else
                        {
                            status.Countdown = 0;
                        }

                        status.StageName = stage.Name;
                        status.StageTypeEnum = stage.Type;
                    }

                    // has the status changed?
                    MetaEventStatus oldStatus = m_TimerData.Events[m_StatusListMap[meta.Id]];
                    if (oldStatus.StageId != status.StageId)
                    {
                        changedStatuses[m_StatusListMap[meta.Id]] = status;
                    }
                }

                if (changedStatuses.Count > 0)
                {
                    LogLine("Status changed on {0} events, acquiring lock...", changedStatuses.Count);

                    bool lockTaken = false;
                    try
                    {
                        m_TimerDataLock.Enter(ref lockTaken);
                        LogLine("Lock acquired, updating and exporting...");

                        m_TimerData.Timestamp = timestamp;

                        foreach (KeyValuePair<int, MetaEventStatus> status in changedStatuses)
                            m_TimerData.Events[status.Key] = status.Value;

                        using (FileStream stream = m_JsonFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            p_Serializer.WriteObject(stream, m_TimerData);
                        }

                        LogLine("Updated, releasing lock...");
                    }
                    catch { }
                    finally
                    {
                        if (lockTaken)
                        {
                            m_TimerDataLock.Exit();
                            LogLine("Lock released");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // log an exception so it can be fixed
                LogLine("Exception thrown in worker thread: {0}", ex.Message);
            }

            // reset sync to 0
            Interlocked.Exchange(ref m_TimerSync, 0);

            LogLine("Worker thread complete");
        }

        private void ResetTimers(int buildId)
        {
            LogLine("Resetting Timers for build {0}, acquiring lock...", buildId);

            bool lockTaken = false;
            try
            {
                m_TimerDataLock.Enter(ref lockTaken);

                m_TimerData.Build = buildId;
                m_TimerData.Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                m_TimerData.Events = new List<MetaEventStatus>();

                foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                {
                    m_TimerData.Events.Add(new MetaEventStatus()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        Countdown = 0,
                        StageId = -1,
                        StageTypeEnum = MetaEventStage.StageType.Reset,
                        StageName = null,
                        Timestamp = m_TimerData.Timestamp
                    });
                }

                m_StatusListMap = new Dictionary<string, int>();
                for (int i = 0; i < m_TimerData.Events.Count; i++)
                {
                    MetaEventStatus status = m_TimerData.Events[i];
                    m_StatusListMap[status.Id] = i;
                }
            }
            catch { }
            finally
            {
                if (lockTaken)
                    m_TimerDataLock.Exit();
            }

            LogLine("Timer reset for build {0} complete", buildId);
        }

        private void LogLine(string message = null, params object[] args)
        {
            // only log to an existing file
            if (!m_LogFile.Exists) return;

            try
            {
                using (FileStream stream = m_LogFile.Open(FileMode.Append, FileAccess.Write))
                {
                    StreamWriter writer = new StreamWriter(stream);

                    if (string.IsNullOrWhiteSpace(message))
                        writer.WriteLine();
                    else
                        writer.WriteLine("[{0}] - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            string.Format(message, args));

                    writer.Close();
                }
            }
            catch { }
        }

        [DataContract]
        private class EventTimerData
        {
            [DataMember(Name = "build")]
            public int Build;

            [DataMember(Name = "timestamp")]
            public long Timestamp;

            [DataMember(Name = "events")]
            public List<MetaEventStatus> Events;
        }

        [DataContract]
        private class MetaEventStatus
        {
            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "minCountdown")]
            public uint MinCountdown { get; set; }

            [DataMember(Name = "maxCountdown")]
            public uint MaxCountdown { get; set; }

            public uint Countdown
            {
                set
                {
                    MinCountdown = value;
                    MaxCountdown = value;
                }
            }

            [DataMember(Name = "stageId")]
            public int StageId { get; set; }

            [DataMember(Name = "stageName")]
            public string StageName { get; set; }

            [DataMember(Name = "stageType")]
            public string StageType { get; set; }

            public MetaEventStage.StageType StageTypeEnum
            {
                get
                {
                    return (MetaEventStage.StageType)Enum.Parse(typeof(MetaEventStage.StageType), StageType, true);
                }
                set
                {
                    StageType = Enum.GetName(typeof(MetaEventStage.StageType), value).ToLower();
                }
            }

            [DataMember(Name = "timestamp")]
            public long Timestamp { get; set; }
        }
    }
}
