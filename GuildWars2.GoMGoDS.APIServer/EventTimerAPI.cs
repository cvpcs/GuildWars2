using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Timers;
using TimeoutException = System.TimeoutException;
using Timer = System.Timers.Timer;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.Model;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class EventTimerAPI : IAPI
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(EventTimerAPI));

        private static TimeSpan p_PollRate = new TimeSpan(0, 0, 30);
        private static DataContractJsonSerializer p_Serializer = new DataContractJsonSerializer(typeof(EventTimerData));

        private Timer m_Timer;
        private int m_TimerSync;

        private EventTimerData m_TimerData = null;
        private SpinLock m_TimerDataLock = new SpinLock();
        private IDictionary<string, int> m_StatusListMap = null;

        private IDbConnection m_DbConn;

        public HttpJsonServer.RequestHandler RequestHandler { get { return GetJson; } }

        public void Start(IDbConnection dbConn)
        {
            LOGGER.Debug("Starting API");

            m_DbConn = dbConn;
            m_TimerData = new EventTimerData();

            LoadDatabase();

            BuildResponse build = new BuildRequest().Execute();
            int buildId = (build != null ? build.BuildId : -1);
            if (m_TimerData == null || m_TimerData.Build != buildId)
                ResetTimers(buildId);

            // start the timer
            m_TimerSync = 0;
            m_Timer = new Timer(p_PollRate.TotalMilliseconds);
            m_Timer.Elapsed += WorkerThread;
            m_Timer.Start();
        }

        public void Stop()
        {
            LOGGER.Debug("Stopping API");

            // stop timer
            m_Timer.Stop();

            // wait for any existing threads to complete
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref m_TimerSync, -1, 0) == 0);

            // zero out our maps
            m_StatusListMap = null;
            m_TimerData = null;
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
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to serialize JSON", e);
            }
            finally
            {
                if (lockTaken)
                    m_TimerDataLock.Exit();
            }

            return data;
        }

        private void LoadDatabase()
        {
            LOGGER.Debug("Loading timer data from the database");

            IDbCommand cmd = m_DbConn.CreateCommand();
            IDbTransaction trns = m_DbConn.BeginTransaction();

            cmd.Connection = m_DbConn;
            cmd.Transaction = trns;

            try
            {
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS eventtimerapi_prop (
                                        key TEXT PRIMARY KEY,
                                        value TEXT)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS eventtimerapi_events (
                                        id TEXT PRIMARY KEY,
                                        name TEXT,
                                        mincountdown INTEGER,
                                        maxcountdown INTEGER,
                                        stageid INTEGER,
                                        stagename TEXT,
                                        stagetype TEXT,
                                        timestamp INTEGER)";
                cmd.ExecuteNonQuery();

                trns.Commit();
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to create tables", e);

                try
                {
                    trns.Rollback();
                }
                catch (Exception ex)
                {
                    LOGGER.Error("Exception thrown when attempting to roll back table creation", ex);
                }
            }

            cmd = m_DbConn.CreateCommand();

            int build = 0;
            long timestamp;

            try
            {
                cmd.CommandText = "SELECT value FROM eventtimerapi_prop WHERE key = @key";
                cmd.AddParameter("@key", "build");
                if (!int.TryParse((string)cmd.ExecuteScalar(), out build))
                    build = 0;
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to select build number", e);
            }

            ResetTimers(build);

            try
            {
                cmd.Parameters.Clear();
                cmd.AddParameter("@key", "timestamp");
                if (long.TryParse((string)cmd.ExecuteScalar(), out timestamp))
                    m_TimerData.Timestamp = timestamp;
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to select timestamp", e);
            }

            try
            {
                cmd.CommandText = "SELECT * FROM eventtimerapi_events";
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MetaEventStatus status = new MetaEventStatus();
                    status.Id = reader["id"].ToString();
                    status.Name = reader["name"].ToString();
                    status.MinCountdown = uint.Parse(reader["mincountdown"].ToString());
                    status.MaxCountdown = uint.Parse(reader["maxcountdown"].ToString());
                    status.StageId = int.Parse(reader["stageid"].ToString());
                    status.StageName = reader["stagename"].ToString();
                    status.StageType = reader["stagetype"].ToString();
                    status.Timestamp = long.Parse(reader["timestamp"].ToString());

                    if (m_StatusListMap.ContainsKey(status.Id))
                    {
                        int i = m_StatusListMap[status.Id];
                        m_TimerData.Events[i] = status;
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to select event data", e);
            }
        }

        private void SaveDatabase()
        {
            LOGGER.Debug("Saving timer data to the database");

            IDbCommand cmd = m_DbConn.CreateCommand();
            IDbTransaction trns = m_DbConn.BeginTransaction();

            cmd.Connection = m_DbConn;
            cmd.Transaction = trns;

            try
            {
                cmd.CommandText = @"INSERT OR REPLACE INTO eventtimerapi_prop (key, value) VALUES ('build', @build), ('timestamp', @timestamp)";
                cmd.AddParameter("@build", m_TimerData.Build.ToString());
                cmd.AddParameter("@timestamp", m_TimerData.Timestamp.ToString());
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"INSERT OR REPLACE INTO eventtimerapi_events (id, name, mincountdown, maxcountdown, stageid, stagename, stagetype, timestamp)
                                        VALUES (@id, @name, @mincountdown, @maxcountdown, @stageid, @stagename, @stagetype, @timestamp)";
                
                foreach (MetaEventStatus status in m_TimerData.Events)
                {
                    cmd.Parameters.Clear();
                    cmd.AddParameter("@id", status.Id);
                    cmd.AddParameter("@name", status.Name);
                    cmd.AddParameter("@mincountdown", status.MinCountdown);
                    cmd.AddParameter("@maxcountdown", status.MaxCountdown);
                    cmd.AddParameter("@stageid", status.StageId);
                    cmd.AddParameter("@stagename", status.StageName);
                    cmd.AddParameter("@stagetype", status.StageType);
                    cmd.AddParameter("@timestamp", status.Timestamp);
                    cmd.ExecuteNonQuery();
                }

                trns.Commit();
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to save timer data to the database", e);

                try
                {
                    trns.Rollback();
                }
                catch (Exception ex)
                {
                    LOGGER.Error("Exception thrown when attempting to roll back timer data save", ex);
                }
            }
        }

        private void ResetTimers(int buildId)
        {
            LOGGER.Debug("Resetting timer data");

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
            catch(Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to reset timer data", e);
            }
            finally
            {
                if (lockTaken)
                    m_TimerDataLock.Exit();
            }
        }

        private void WorkerThread(object sender, ElapsedEventArgs e)
        {
            // attempt to set the sync, if another of us is running, just exit
            if (Interlocked.CompareExchange(ref m_TimerSync, 1, 0) != 0)
                return;

            LOGGER.Debug("Worker thread process beginning");

            // wrap in a try-catch so we can release our interlock if something fails
            try
            {
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
                    int stageId = meta.GetStageId(metaEvents, m_TimerData.Events[m_StatusListMap[meta.Id]].StageId);

                    // stock state
                    MetaEventStatus status = new MetaEventStatus()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        MinCountdown = meta.MinSpawn,
                        MaxCountdown = meta.MaxSpawn,
                        StageId = stageId,
                        StageType = null,
                        StageName = null,
                        Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds
                    };

                    // we're in a stage
                    if (stageId >= 0)
                    {
                        MetaEventStage stage = meta.Stages[stageId];

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
                    bool lockTaken = false;
                    try
                    {
                        m_TimerDataLock.Enter(ref lockTaken);

                        m_TimerData.Timestamp = timestamp;

                        foreach (KeyValuePair<int, MetaEventStatus> status in changedStatuses)
                            m_TimerData.Events[status.Key] = status.Value;

                        SaveDatabase();
                    }
                    catch (Exception exi)
                    {
                        LOGGER.Error("Exception thrown when saving database in worker thread", exi);
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            m_TimerDataLock.Exit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("Exception thrown in worker thread", ex);
            }

            LOGGER.Debug("Worker thread process completed");

            // reset sync to 0
            Interlocked.Exchange(ref m_TimerSync, 0);
        }
    }
}
