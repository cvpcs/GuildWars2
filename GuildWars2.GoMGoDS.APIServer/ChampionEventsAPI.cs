using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.XPath;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.GoMGoDS.API;
using GuildWars2.GoMGoDS.Model;

using HtmlAgilityPack;
using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class ChampionEventsAPI : CachedAPI<ChampionEventsResponse>
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(ChampionEventsAPI));

        private static string WIKI_URL = "http://wiki.guildwars2.com/wiki/List_of_champions";
        private static Regex WIKI_NAME_CLEAN = new Regex("[^a-z0-9]");
        private static Regex GUID = new Regex("[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}");

        private static TimeSpan p_CacheTimeout = new TimeSpan(24, 0, 0);

        private IDbConnection m_DbConn;
        private DateTime m_CacheTimestamp = DateTime.MinValue;

        public ChampionEventsAPI()
            : base(p_CacheTimeout)
        { }

        #region APIBase
        public override string RequestPath { get { return "/championevents.json"; } }

        public override void Init(IDbConnection dbConn)
        {
            m_DbConn = dbConn;
            DbCreateTables();
        }

        protected override ChampionEventsResponse GetData(IDictionary<string, string> _get)
        {
            ChampionEventsResponse data = new ChampionEventsResponse();
            data.ChampionEvents = DbGetEvents();

            return data;
        }
        #endregion

        #region CachedAPI
        protected override void RefreshData(IDictionary<string, string> _get)
        {
            string html;

            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(WIKI_URL);
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            XPathNavigator nav = doc.CreateNavigator();
            XPathNodeIterator i = nav.Select("//table/tr/td[position()=6]");

            IList<string> champ_list = new List<string>();

            while (i.MoveNext())
            {
                XPathNavigator node = i.Current.SelectSingleNode("./a");

                if (node != null)
                    champ_list.Add(WIKI_NAME_CLEAN.Replace(node.Value.ToLower(), string.Empty));
            }

            EventNamesResponse names = new EventNamesRequest(LanguageCode.EN).Execute();

            IDbTransaction tx = m_DbConn.BeginTransaction();

            try
            {
                foreach (Event ev in names)
                {
                    if (champ_list.Contains(WIKI_NAME_CLEAN.Replace(ev.Name.ToLower(), string.Empty)))
                        DbAddEvent(ev.Id, tx);
                }

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
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS championeventsapi_events (
                                        id TEXT PRIMARY KEY)";
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

        private List<Guid> DbGetEvents()
        {
            List<Guid> events = new List<Guid>();

            IDbCommand cmd = m_DbConn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM championeventsapi_events";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        events.Add(new Guid(reader["id"].ToString()));
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to get event data", e);
            }

            return events;
        }

        private void DbAddEvent(Guid event_id, IDbTransaction tx = null)
        {
            IDbCommand cmd = m_DbConn.CreateCommand();

            if (tx != null)
            {
                cmd.Connection = m_DbConn;
                cmd.Transaction = tx;
            }

            try
            {
                cmd.CommandText = @"INSERT OR REPLACE INTO championeventsapi_events (id) VALUES (@id)";
                cmd.AddParameter("@id", event_id.ToString());
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LOGGER.Error(string.Format("Exception thrown when attempting to add event [{0}]", event_id.ToString()), e);
            }
        }
        #endregion
    }
}
