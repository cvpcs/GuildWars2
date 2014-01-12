using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public abstract class PublisherBase<T> : IPublisher<T>
    {
        private static ILog LOGGER = LogManager.GetLogger(typeof(PublisherBase<>));

        private TimeSpan m_PollRate;
        private Timer m_Timer;
        private int m_TimerSync;

        private IList<ISubscriber<T>> m_Subscribers;

        protected T m_Data;

        public PublisherBase(TimeSpan pollRate)
        {
            m_PollRate = pollRate;

            m_Subscribers = new List<ISubscriber<T>>();
        }

        public void Start()
        {
            LOGGER.Debug("Starting publisher");

            // start the timer
            m_TimerSync = 0;
            m_Timer = new Timer(m_PollRate.TotalMilliseconds);
            m_Timer.Elapsed += WorkerThread;
            m_Timer.Start();
            
            // force-call the timer at least once
            WorkerThread(this, null);
        }

        public void Stop()
        {
            LOGGER.Debug("Stopping publisher");

            // stop timer
            m_Timer.Stop();

            // wait for any existing threads to complete
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref m_TimerSync, -1, 0) == 0);
        }

        public void RegisterSubscriber(ISubscriber<T> subscriber)
        {
            if (!m_Subscribers.Contains(subscriber))
                m_Subscribers.Add(subscriber);
        }

        protected abstract bool UpdateData();

        private void WorkerThread(object sender, ElapsedEventArgs e)
        {
            // attempt to set the sync, if another of us is running, just exit
            if (Interlocked.CompareExchange(ref m_TimerSync, 1, 0) != 0)
                return;

            LOGGER.Debug("Worker thread process beginning");

            // wrap in a try-catch so we can release our interlock if something fails
            try
            {
                if (UpdateData())
                {
                    foreach (ISubscriber<T> subscriber in m_Subscribers)
                        subscriber.Process(m_Data);
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
