using System;
using System.Data;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public abstract class TimerAPI : IAPI
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(TimerAPI));

        private TimeSpan m_PollRate;
        private Timer m_Timer;
        private int m_TimerSync;

        public abstract HttpJsonServer.RequestHandler RequestHandler { get; }

        public TimerAPI(TimeSpan pollRate)
        {
            m_PollRate = pollRate;
        }

        public void Start(IDbConnection dbConn)
        {
            Setup(dbConn);

            LOGGER.Debug("Starting timer");

            // start the timer
            m_TimerSync = 0;
            m_Timer = new Timer(m_PollRate.TotalMilliseconds);
            m_Timer.Elapsed += WorkerThread;
            m_Timer.Start();
            
            // force-call the thread at least once
            WorkerThread(this, null);
        }

        public void Stop()
        {
            LOGGER.Debug("Stopping timer");

            // stop timer
            m_Timer.Stop();

            // wait for any existing threads to complete
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref m_TimerSync, -1, 0) == 0);

            Cleanup();
        }

        protected abstract void Setup(IDbConnection dbConn);
        protected abstract void Cleanup();
        protected abstract void Run();

        private void WorkerThread(object sender, ElapsedEventArgs e)
        {
            // attempt to set the sync, if another of us is running, just exit
            if (Interlocked.CompareExchange(ref m_TimerSync, 1, 0) != 0)
                return;

            LOGGER.Debug("Worker thread process beginning");

            // wrap in a try-catch so we can release our interlock if something fails
            try
            {
                Run();
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
