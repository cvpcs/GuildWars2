using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace GuildWars2.ArenaNet.EventTimer
{
    public class HttpJsonServer : IDisposable
    {
        private readonly HttpListener m_Listener;
        private readonly Thread m_ListenerThread;
        private readonly Thread[] m_WorkerThreads;
        private readonly ManualResetEvent m_Stop;
        private readonly ManualResetEvent m_Ready;
        private Queue<HttpListenerContext> m_Queue;

        public delegate string RequestHandler();
        public event RequestHandler OnRequestReceived;

        public HttpJsonServer(uint port)
        {
            if (!HttpListener.IsSupported)
                throw new NotImplementedException("HttpListener is not supported");

            m_Listener = new HttpListener();
            m_Listener.Prefixes.Add(string.Format("http://localhost:{0}/", port));

            m_ListenerThread = new Thread(HandleRequests);
            m_WorkerThreads = new Thread[5];
            m_Stop = new ManualResetEvent(false);
            m_Ready = new ManualResetEvent(false);
            m_Queue = new Queue<HttpListenerContext>();
        }

        public void Start()
        {
            m_Listener.Start();
            m_ListenerThread.Start();

            for (int i = 0; i < m_WorkerThreads.Length; i++)
            {
                m_WorkerThreads[i] = new Thread(Worker);
                m_WorkerThreads[i].Start();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            m_Stop.Set();

            m_ListenerThread.Join();
            foreach (Thread worker in m_WorkerThreads)
                worker.Join();

            m_Listener.Stop();
        }

        private void HandleRequests()
        {
            while (m_Listener.IsListening)
            {
                IAsyncResult result = m_Listener.BeginGetContext(ContextReady, null);

                // wait for a request or the indication to stop
                if (WaitHandle.WaitAny(new WaitHandle[] { m_Stop, result.AsyncWaitHandle }) == 0)
                    break;
            }
        }

        private void ContextReady(IAsyncResult result)
        {
            try
            {
                lock (m_Queue)
                {
                    m_Queue.Enqueue(m_Listener.EndGetContext(result));
                    m_Ready.Set();
                }
            }
            catch { }
        }

        private void Worker()
        {
            while (WaitHandle.WaitAny(new WaitHandle[] { m_Ready, m_Stop }) == 0)
            {
                HttpListenerContext ctx;
                
                lock (m_Queue)
                {
                    if (m_Queue.Count > 0)
                        ctx = m_Queue.Dequeue();
                    else
                    {
                        m_Ready.Reset();
                        continue;
                    }
                }
                
                if (OnRequestReceived != null)
                {
                    HttpListenerRequest request = ctx.Request;

                    string str = OnRequestReceived();
                    byte[] buf = Encoding.UTF8.GetBytes(str);

                    HttpListenerResponse response = ctx.Response;
                    response.ContentType = "application/json";
                    response.ContentLength64 = buf.Length;

                    Stream output = response.OutputStream;
                    output.Write(buf, 0, buf.Length);
                    output.Close();
                }
            }
        }
    }
}
