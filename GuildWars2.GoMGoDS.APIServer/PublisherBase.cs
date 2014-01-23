using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public abstract class PublisherBase<T> : IPublisher<T>
    {
        private static ILog LOGGER = null;

        private TimeSpan m_PollRate;
        private Thread m_WorkerThread;
        private ManualResetEventSlim m_WorkerThreadCancelled;

        private IList<ISubscriber<T>> m_Subscribers;

        protected T m_Data;

        public PublisherBase(TimeSpan pollRate)
        {
            if (LOGGER == null)
                LOGGER = LogManager.GetLogger(this.GetType());

            m_PollRate = pollRate;

            m_Subscribers = new List<ISubscriber<T>>();
        }

        public void Start()
        {
            LOGGER.Debug("Starting publisher");

            // start the worker thread
            m_WorkerThreadCancelled = new ManualResetEventSlim();
            m_WorkerThread = new Thread(WorkerThread);
            m_WorkerThread.Start();
        }

        public void Stop()
        {
            LOGGER.Debug("Stopping publisher");

            // wait for the thread to exit
            m_WorkerThreadCancelled.Set();
            m_WorkerThread.Join();
        }

        public void RegisterSubscriber(ISubscriber<T> subscriber)
        {
            if (!m_Subscribers.Contains(subscriber))
                m_Subscribers.Add(subscriber);
        }

        protected abstract bool UpdateData();

        private void WorkerThread()
        {
            while (!m_WorkerThreadCancelled.IsSet)
            {
                LOGGER.Debug("Worker thread process beginning");
                DateTime processingBegin = DateTime.UtcNow;

                // wrap in a try-catch to make sure nothing goofy happens
                try
                {
                    if (UpdateData())
                    {
                        LOGGER.Debug("Processing subscribers...");

                        foreach (ISubscriber<T> subscriber in m_Subscribers)
                        {
                            LOGGER.DebugFormat("Calling {0} for processing...", subscriber.GetType().Name);
                            subscriber.Process(m_Data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOGGER.Error("Exception thrown in worker thread", ex);
                }

                LOGGER.Debug("Worker thread process completed");

                // sleep for the appropriate amount of time
                TimeSpan waitTime = m_PollRate - (DateTime.UtcNow - processingBegin);
                if (waitTime > TimeSpan.Zero)
                    m_WorkerThreadCancelled.Wait(waitTime);
            }
        }
    }
}
