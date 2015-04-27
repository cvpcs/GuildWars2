using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using GuildWars2.ArenaNet.API.V1;
using GuildWars2.ArenaNet.Model.V1;
using GuildWars2.GoMGoDS.API;
using GuildWars2.GoMGoDS.Model;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class ContestedStatusAPI : APIBase<ContestedStatusResponse>, ISubscriber<BuildResponse>, ISubscriber<EventsResponse>
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(ContestedStatusAPI));

        private IDbConnection m_DbConn;

        #region APIBase
        public override string RequestPath { get { return "/contestedstatus.json"; } }

        public override void Init(IDbConnection dbConn)
        {
            m_DbConn = dbConn;
            DbCreateTables();
        }

        protected override ContestedStatusResponse GetData(IDictionary<string, string> _get)
        {
            ContestedStatusResponse data = new ContestedStatusResponse()
            {
                Build = DbGetBuild(),
                Timestamp = DbGetTimestamp(),
                Locations = new List<ContestedLocationStatus>()
            };

            foreach (ContestedLocation loc in ContestedLocationDefinitions.ContestedLocations)
                data.Locations.Add(DbGetContestedLocationStatus(loc.Id));

            return data;
        }
        #endregion

        #region ISubscriber
        public void Process(BuildResponse build)
        {
            if (build.BuildId != DbGetBuild())
            {
                LOGGER.Debug("Resetting contested data");

                long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                IDbTransaction tx = m_DbConn.BeginTransaction();

                try
                {
                    DbSetProperty("build", build.BuildId.ToString(), tx);
                    DbSetProperty("timestamp", timestamp.ToString(), tx);

                    foreach (ContestedLocation loc in ContestedLocationDefinitions.ContestedLocations)
                    {
                        DbSetContestedLocationStatus(new ContestedLocationStatus()
                        {
                            Name = loc.Name,
                            Abbreviation = loc.Abbreviation,
                            OpenOn = new List<int>(),
                            DefendOn = new List<int>(),
                            CaptureOn = new List<int>()
                        }, tx);
                    }

                    tx.Commit();
                }
                catch (Exception e)
                {
                    LOGGER.Error("Exception thrown when attempting to reset contested locations", e);

                    try
                    {
                        tx.Rollback();
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error("Exception thrown when attempting to roll back constested location reset", ex);
                    }
                }
            }
        }

        public void Process(EventsResponse events)
        {
            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            HashSet<EventState> contestedLocationEvents = new HashSet<EventState>(events.Events.Where(es => ContestedLocationDefinitions.EventList.Contains(es.EventId)));
            IList<ContestedLocationStatus> changedStatuses = new List<ContestedLocationStatus>();

            foreach (ContestedLocation loc in ContestedLocationDefinitions.ContestedLocations)
                changedStatuses.Add(loc.GetStatus(contestedLocationEvents));

            if (changedStatuses.Count > 0)
            {
                LOGGER.Debug("Saving updated contested location status(es)");

                IDbTransaction tx = m_DbConn.BeginTransaction();

                try
                {
                    DbSetProperty("timestamp", timestamp.ToString(), tx);

                    foreach (ContestedLocationStatus status in changedStatuses)
                        DbSetContestedLocationStatus(status, tx);

                    tx.Commit();
                }
                catch (Exception e)
                {
                    LOGGER.Error("Exception thrown when attempting to update contested location status", e);

                    try
                    {
                        tx.Rollback();
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error("Exception thrown when attempting to roll back contested location status update", ex);
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
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS contestedstatusapi_prop (
                                        key TEXT PRIMARY KEY,
                                        value TEXT)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS contestedstatusapi_open (
                                        location_id TEXT,
                                        world_id INTEGER,
                                        PRIMARY KEY (location_id, world_id))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS contestedstatusapi_defend (
                                        location_id TEXT,
                                        world_id INTEGER,
                                        PRIMARY KEY (location_id, world_id))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS contestedstatusapi_capture (
                                        location_id TEXT,
                                        world_id INTEGER,
                                        PRIMARY KEY (location_id, world_id))";
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
                cmd.CommandText = "SELECT value FROM contestedstatusapi_prop WHERE key = @key";
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
                cmd.CommandText = @"INSERT OR REPLACE INTO contestedstatusapi_prop (key, value) VALUES (@key, @value)";
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

        private ContestedLocationStatus DbGetContestedLocationStatus(string id)
        {
            ContestedLocation loc = ContestedLocationDefinitions.ContestedLocations.Where(l => l.Id == id).FirstOrDefault();
            if (loc == null)
                throw new KeyNotFoundException(string.Format("Contested location [{0}] does not exist", id));

            ContestedLocationStatus status = new ContestedLocationStatus()
                {
                    Name = loc.Name,
                    Abbreviation = loc.Abbreviation,
                    OpenOn = new List<int>(),
                    DefendOn = new List<int>(),
                    CaptureOn = new List<int>()
                };

            IDbCommand cmd = m_DbConn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT world_id FROM contestedstatusapi_open WHERE location_id = @id";
                cmd.AddParameter("@id", id);
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        status.OpenOn.Add(int.Parse(reader["world_id"].ToString()));
                }

                cmd.CommandText = "SELECT world_id FROM contestedstatusapi_defend WHERE location_id = @id";
                cmd.AddParameter("@id", id);
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        status.DefendOn.Add(int.Parse(reader["world_id"].ToString()));
                }

                cmd.CommandText = "SELECT world_id FROM contestedstatusapi_capture WHERE location_id = @id";
                cmd.AddParameter("@id", id);
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        status.CaptureOn.Add(int.Parse(reader["world_id"].ToString()));
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to get contested location data [{0}]", id), e);
            }

            return status;
        }

        private void DbSetContestedLocationStatus(ContestedLocationStatus status, IDbTransaction tx = null)
        {
            IDbCommand cmd = m_DbConn.CreateCommand();

            if (tx != null)
            {
                cmd.Connection = m_DbConn;
                cmd.Transaction = tx;
            }

            try
            {
                cmd.CommandText = @"DELETE FROM contestedstatusapi_open WHERE location_id = @id";
                cmd.AddParameter("@id", status.Id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"DELETE FROM contestedstatusapi_defend WHERE location_id = @id";
                cmd.AddParameter("@id", status.Id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"DELETE FROM contestedstatusapi_capture WHERE location_id = @id";
                cmd.AddParameter("@id", status.Id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"INSERT OR REPLACE INTO contestedstatusapi_open (location_id, world_id) VALUES (@id, @world_id)";
                cmd.AddParameter("@id", status.Id);

                foreach (int world_id in status.OpenOn)
                {
                    cmd.AddParameter("@world_id", world_id);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = @"INSERT OR REPLACE INTO contestedstatusapi_defend (location_id, world_id) VALUES (@id, @world_id)";
                cmd.AddParameter("@id", status.Id);

                foreach (int world_id in status.DefendOn)
                {
                    cmd.AddParameter("@world_id", world_id);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = @"INSERT OR REPLACE INTO contestedstatusapi_capture (location_id, world_id) VALUES (@id, @world_id)";
                cmd.AddParameter("@id", status.Id);

                foreach (int world_id in status.CaptureOn)
                {
                    cmd.AddParameter("@world_id", world_id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to set contested location data [{0}]", status.Abbreviation), e);
            }
        }
        #endregion
    }
}
