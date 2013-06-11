using System;
using System.Collections.Generic;
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
using System.Threading.Tasks;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.EventTimer
{
    public partial class EventTimerService : ServiceBase
    {
        private static TimeSpan p_PollRate = new TimeSpan(0, 0, 30);

        private static DataContractJsonSerializer p_Serializer = new DataContractJsonSerializer(typeof(EventTimerData));
        
        private FileInfo m_JsonFile = new FileInfo("event_timer.json");
        private EventTimerData m_TimerData = null;
        private IDictionary<string, int> m_StatusListMap = null;

        private Task m_WorkerTask;
        private bool m_Running = false;

        public EventTimerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // load up our database
            if (m_JsonFile.Exists)
            {
                FileStream stream = null;
                try
                {
                    stream = m_JsonFile.Open(FileMode.Open, FileAccess.Read);
                    m_TimerData = p_Serializer.ReadObject(stream) as EventTimerData;
                    m_StatusListMap = new Dictionary<string, int>();
                    for (int i = 0; i < m_TimerData.Events.Count; i++)
                    {
                        MetaEventStatus status = m_TimerData.Events[i];
                        m_StatusListMap[status.Id] = i;
                    }
                }
                catch (Exception)
                { }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }

            int buildId = new BuildRequest().Execute().BuildId;
            if (m_TimerData == null || m_TimerData.Build != buildId)
            {
                m_TimerData = new EventTimerData();
                ResetTimers(buildId);
            }

            // start the worker thread
            m_Running = true;
            m_WorkerTask = new Task(WorkerThread);
            m_WorkerTask.Start();
        }

        protected override void OnStop()
        {
            // wait for the worker thread to stop
            m_Running = false;
            m_WorkerTask.Wait();

            // zero out our maps
            m_StatusListMap = null;
            m_TimerData = null;
        }

        private void WorkerThread()
        {
            while (m_Running)
            {
                // get current time
                DateTime startTime = DateTime.Now;

                // check if build has changed
                int buildId = new BuildRequest().Execute().BuildId;
                if (buildId != m_TimerData.Build)
                    ResetTimers(buildId);

                // get data
                EventsResponse response = new EventsRequest(1007).Execute();
                IList<EventState> metaEvents = response.Events.Where(es => MetaEventDefinitions.EventList.Contains(es.EventId)).ToList();

                m_TimerData.Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                bool stateChanged = false;

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
                        m_TimerData.Events[m_StatusListMap[meta.Id]] = status;
                        stateChanged = true;
                    }
                }
                
                if (stateChanged)
                {
                    // write to database
                    FileStream stream = null;
                    try
                    {
                        stream = m_JsonFile.Open(FileMode.Create, FileAccess.Write, FileShare.None);
                        p_Serializer.WriteObject(stream, m_TimerData);
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }

                // sleep until we hit our poll rate
                TimeSpan calcSpan = DateTime.Now - startTime;
                if (calcSpan < p_PollRate)
                    Thread.Sleep(p_PollRate - calcSpan);
            }
        }

        private void ResetTimers(int buildId)
        {
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
