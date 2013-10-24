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

namespace GuildWars2.GoMGoDS.APIServer
{
    public partial class APIServer : ServiceBase
    {

        private IDictionary<string, IAPI> m_APIs = new Dictionary<string, IAPI>();
        private HttpJsonServer m_JsonServer = new HttpJsonServer(uint.Parse(ConfigurationManager.AppSettings["port"]));
        private IDbConnection m_DbConn;

        public APIServer()
        {
            InitializeComponent();

            m_APIs["/event_timer_data.json"] = new EventTimerAPI();
        }

        protected override void OnStart(string[] args)
        {
            m_DbConn = new SqliteConnection(string.Format("Data Source=file:{0}", ConfigurationManager.AppSettings["sqlite_db"]));
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
            m_JsonServer.Stop();

            foreach (IAPI api in m_APIs.Values)
            {
                api.Stop();
            }

            m_DbConn.Close();
        }
    }
}
