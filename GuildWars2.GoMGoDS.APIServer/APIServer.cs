using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Mono.Data.Sqlite;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public partial class APIServer : ServiceBase
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(APIServer));

        private IDictionary<string, IAPI> m_APIs = new Dictionary<string, IAPI>();
        private HttpJsonServer m_JsonServer = new HttpJsonServer(uint.Parse(ConfigurationManager.AppSettings["port"]));
        private IDbConnection m_DbConn;

        public APIServer()
        {
            InitializeComponent();

            m_APIs["/championevents.json"] = new ChampionEventsAPI();
            m_APIs["/eventtimer.json"] = new EventTimerAPI();
            m_APIs["/nodes.json"] = new NodesAPI();
        }

        protected override void OnStart(string[] args)
        {
            LOGGER.Debug("Starting service");

            m_DbConn = new SqliteConnection(string.Format("Data Source={0}", ConfigurationManager.AppSettings["sqlite_db"]));
            m_DbConn.Open();

            foreach (string path in m_APIs.Keys)
            {
                IAPI api = m_APIs[path];

                m_JsonServer.RegisterPath(path, api.RequestHandler);

                api.Start(m_DbConn);
            }

            m_JsonServer.Start();
        }

        protected override void OnStop()
        {
            LOGGER.Debug("Stopping service");

            m_JsonServer.Stop();

            foreach (IAPI api in m_APIs.Values)
            {
                api.Stop();
            }

            m_DbConn.Close();
        }
    }
}
