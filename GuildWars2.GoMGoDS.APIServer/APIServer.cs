using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;
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

        private IList<IPublisher> m_Publishers = new List<IPublisher>();
        private IList<IAPI> m_APIs = new List<IAPI>();

        private HttpJsonServer m_JsonServer = new HttpJsonServer(uint.Parse(ConfigurationManager.AppSettings["port"]));
        private IDbConnection m_DbConn;

        public APIServer()
        {
            InitializeComponent();

            // set up out publishers
            BuildIdPublisher buildIdPublisher = new BuildIdPublisher();
            EventStatePublisher eventStatePublisher = new EventStatePublisher();

            m_Publishers.Add(buildIdPublisher);
            m_Publishers.Add(eventStatePublisher);

            // set up our APIs
            ChampionEventsAPI championEventsApi = new ChampionEventsAPI();
            EventTimerAPI eventTimerApi = new EventTimerAPI();
            NodesAPI nodesApi = new NodesAPI();

            m_APIs.Add(championEventsApi);
            m_APIs.Add(eventTimerApi);
            m_APIs.Add(nodesApi);

            // register our subscribers
            buildIdPublisher.RegisterSubscriber(eventTimerApi);
            eventStatePublisher.RegisterSubscriber(eventTimerApi);
        }

        protected override void OnStart(string[] args)
        {
            LOGGER.Debug("Starting service");

            m_DbConn = new SqliteConnection(string.Format("Data Source={0}", ConfigurationManager.AppSettings["sqlite_db"]));
            m_DbConn.Open();

            foreach (IAPI api in m_APIs)
            {
                m_JsonServer.RegisterPath(api.RequestPath, api.RequestHandler);
                api.Init(m_DbConn);
            }

            foreach (IPublisher publisher in m_Publishers)
                publisher.Start();

            m_JsonServer.Start();
        }

        protected override void OnStop()
        {
            LOGGER.Debug("Stopping service");

            m_JsonServer.Stop();

            foreach (IPublisher publisher in m_Publishers)
                publisher.Stop();

            m_DbConn.Close();
        }
    }
}
