using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.GoMGoDS.API;
using GuildWars2.GoMGoDS.Model;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class EventTimerAPI : APIBase<EventTimerResponse>, ISubscriber<BuildResponse>, ISubscriber<EventsResponse>
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(EventTimerAPI));
        private static int WORLD_ID = 1007;

        private IDbConnection m_DbConn;

        #region APIBase
        public override string RequestPath { get { return "/eventtimer.json"; } }

        public override void Init(IDbConnection dbConn)
        {
            m_DbConn = dbConn;
            DbCreateTables();
        }

        protected override EventTimerResponse GetData(IDictionary<string, string> _get)
        {
            EventTimerResponse data = new EventTimerResponse()
            {
                Build = DbGetBuild(),
                Timestamp = DbGetTimestamp(),
                Events = new List<MetaEventStatus>()
            };

            foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                data.Events.Add(DbGetMetaEventStatus(meta.Id));

            return data;
        }
        #endregion

        #region ISubscriber
        public void Process(BuildResponse build)
        {
            if (build.BuildId != DbGetBuild())
            {
                LOGGER.Debug("Resetting timer data");

                long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                IDbTransaction tx = m_DbConn.BeginTransaction();

                try
                {
                    DbSetProperty("build", build.BuildId.ToString(), tx);
                    DbSetProperty("timestamp", timestamp.ToString(), tx);

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
        }

        public void Process(EventsResponse events)
        {
            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            HashSet<EventState> metaEvents = new HashSet<EventState>(events.Events.Where(es => es.WorldId == WORLD_ID && MetaEventDefinitions.EventList.Contains(es.EventId)));

            HashSet<MetaEventStatus> changedStatuses = new HashSet<MetaEventStatus>();

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
                    MetaEventStage stage = meta.Stages.ElementAt(stageId);

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
                if (oldStatus.StageId != status.StageId || (stageId >= 0 && oldStatus.StageName !=status.StageName))
                {
                    // if the actual stage hasn't changed (multiline scenareo) then don't update the timestamp
                    if (oldStatus.StageId == status.StageId)
                        status.Timestamp = oldStatus.Timestamp;

                    changedStatuses.Add(status);
                }
            }

            if (changedStatuses.Count > 0)
            {
                LOGGER.DebugFormat("Saving {0} updated event status(es)", changedStatuses.Count);

                IDbTransaction tx = m_DbConn.BeginTransaction();

                try
                {
                    DbSetProperty("timestamp", timestamp.ToString(), tx);

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
        #endregion

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

        private string DbGetProperty(string key)
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
                    return obj.ToString();
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to get property [{0}]", key), e);
            }

            return string.Empty;
        }

        private void DbSetProperty(string key, string value, IDbTransaction tx = null)
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
                cmd.AddParameter("@value", value);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to set property [{0} = {1}]", key, value.ToString()), e);
            }
        }

        private int DbGetBuild()
        {
            int build;

            if (!int.TryParse(DbGetProperty("build"), out build))
                build = -1;

            return build;
        }

        private long DbGetTimestamp()
        {
            long timestamp;

            if (!long.TryParse(DbGetProperty("timestamp"), out timestamp))
                timestamp = -1;

            return timestamp;
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
                    Timestamp = DbGetTimestamp()
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
