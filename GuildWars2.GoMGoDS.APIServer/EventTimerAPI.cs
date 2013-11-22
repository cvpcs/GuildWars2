using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.Model;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class EventTimerAPI : TimerAPI
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(EventTimerAPI));

        private static TimeSpan p_PollRate = new TimeSpan(0, 0, 30);
        private static DataContractJsonSerializer p_Serializer = new DataContractJsonSerializer(typeof(EventTimerData));

        private IDbConnection m_DbConn;

        public override HttpJsonServer.RequestHandler RequestHandler { get { return GetJson; } }

        public EventTimerAPI()
            : base(p_PollRate)
        { }

        protected override void Setup(IDbConnection dbConn)
        {
            m_DbConn = dbConn;
            DbCreateTables();
        }

        protected override void Cleanup()
        { }

        protected override void Run()
        {
            // check if build has changed, reset timers if it has
            BuildResponse build = new BuildRequest().Execute();
            if (build != null && build.BuildId != DbGetProperty<int>("build"))
                ResetTimers(build.BuildId);

            // get data
            EventsResponse response = new EventsRequest(1007).Execute();
            if (response == null)
                throw new TimeoutException("Request timed out when attempting to retrieve event data.");

            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            IList<EventState> metaEvents = response.Events.Where(es => MetaEventDefinitions.EventList.Contains(es.EventId)).ToList();

            IList<MetaEventStatus> changedStatuses = new List<MetaEventStatus>();

            // discover meta-event states
            foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
            {
                MetaEventStatus oldStatus = DbGetMetaEventStatus(meta.Id);
                int stageId = meta.GetStageId(metaEvents, oldStatus.StageId);

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
                if (oldStatus.StageId != status.StageId)
                {
                    changedStatuses.Add(status);
                }
            }

            if (changedStatuses.Count > 0)
            {
                IDbTransaction tx = m_DbConn.BeginTransaction();

                try
                {
                    DbSetProperty<long>("timestamp", timestamp, tx);

                    foreach (MetaEventStatus status in changedStatuses)
                        DbSetMetaEventStatus(status, tx);

                    tx.Commit();
                }
                catch (Exception e)
                {
                    LOGGER.Error("Exception thrown when attempting to update event status", e);

                    try
                    {
                        tx.Rollback();
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error("Exception thrown when attempting to roll back event status update", ex);
                    }
                }
            }
        }

        private string GetJson()
        {
            string data = string.Empty;
            EventTimerData timerData = new EventTimerData()
                {
                    Build = DbGetProperty<int>("build"),
                    Timestamp = DbGetProperty<long>("timestamp"),
                    Events = new List<MetaEventStatus>()
                };

            foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                timerData.Events.Add(DbGetMetaEventStatus(meta.Id));

            try
            {
                MemoryStream stream = new MemoryStream();
                p_Serializer.WriteObject(stream, timerData);
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

            return data;
        }

        private void ResetTimers(int build)
        {
            LOGGER.Debug("Resetting timer data");

            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            IDbTransaction tx = m_DbConn.BeginTransaction();

            try
            {
                DbSetProperty<int>("build", build, tx);
                DbSetProperty<long>("timestamp", timestamp, tx);

                foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                {
                    DbSetMetaEventStatus(new MetaEventStatus()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        Countdown = 0,
                        StageId = -1,
                        StageTypeEnum = MetaEventStage.StageType.Reset,
                        StageName = null,
                        Timestamp = timestamp
                    }, tx);
                }

                tx.Commit();
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to reset timers", e);

                try
                {
                    tx.Rollback();
                }
                catch (Exception ex)
                {
                    LOGGER.Error("Exception thrown when attempting to roll back timer reset", ex);
                }
            }
        }

        #region Database
        private void DbCreateTables()
        {
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
        }

        private T DbGetProperty<T>(string key)
        {
            IDbCommand cmd = m_DbConn.CreateCommand();
            cmd = m_DbConn.CreateCommand();;

            try
            {
                cmd.CommandText = "SELECT value FROM eventtimerapi_prop WHERE key = @key";
                cmd.AddParameter("@key", key);

                object obj = cmd.ExecuteScalar();

                if (obj != null)
                {
                    System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(obj);
                    return (T)tc.ConvertTo(obj, typeof(T));
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to get property [{0}]", key), e);
            }

            return default(T);
        }

        private void DbSetProperty<T>(string key, T value, IDbTransaction tx = null)
        {
            IDbCommand cmd = m_DbConn.CreateCommand();

            if (tx != null)
            {
                cmd.Connection = m_DbConn;
                cmd.Transaction = tx;
            }

            try
            {
                cmd.CommandText = @"INSERT OR REPLACE INTO eventtimerapi_prop (key, value) VALUES (@key, @value)";
                cmd.AddParameter("@key", key);
                cmd.AddParameter("@value", value.ToString());
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to set property [{0} = {1}]", key, value.ToString()), e);
            }
        }

        private MetaEventStatus DbGetMetaEventStatus(string id)
        {
            MetaEvent meta = MetaEventDefinitions.MetaEvents.Where(m => m.Id == id).FirstOrDefault();
            if (meta == null)
                throw new KeyNotFoundException(string.Format("Meta event [{0}] does not exist", id));

            MetaEventStatus status = new MetaEventStatus()
                {
                    Id = meta.Id,
                    Name = meta.Name,
                    Countdown = 0,
                    StageId = -1,
                    StageTypeEnum = MetaEventStage.StageType.Reset,
                    StageName = null,
                    Timestamp = DbGetProperty<long>("timestamp")
                };

            IDbCommand cmd = m_DbConn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM eventtimerapi_events WHERE id = @id";
                cmd.AddParameter("@id", id);
                IDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    status.Id = reader["id"].ToString();
                    status.Name = reader["name"].ToString();
                    status.MinCountdown = uint.Parse(reader["mincountdown"].ToString());
                    status.MaxCountdown = uint.Parse(reader["maxcountdown"].ToString());
                    status.StageId = int.Parse(reader["stageid"].ToString());
                    status.StageName = reader["stagename"].ToString();
                    status.StageType = reader["stagetype"].ToString();
                    status.Timestamp = long.Parse(reader["timestamp"].ToString());
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to get event data [{0}]", id), e);
            }

            return status;
        }

        private void DbSetMetaEventStatus(MetaEventStatus status, IDbTransaction tx = null)
        {
            IDbCommand cmd = m_DbConn.CreateCommand();

            if (tx != null)
            {
                cmd.Connection = m_DbConn;
                cmd.Transaction = tx;
            }

            try
            {
                cmd.CommandText = @"INSERT OR REPLACE INTO eventtimerapi_events (id, name, mincountdown, maxcountdown, stageid, stagename, stagetype, timestamp)
                                        VALUES (@id, @name, @mincountdown, @maxcountdown, @stageid, @stagename, @stagetype, @timestamp)";
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
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to set event data [{0}]", status.Id), e);
            }
        }
        #endregion
    }
}
