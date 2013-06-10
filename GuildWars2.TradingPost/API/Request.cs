using System;
using System.Collections.Generic;
using System.Threading;

using RestSharp;

namespace GuildWars2.TradingPost.API
{
    public abstract class Request<T>
        where T : class, new()
    {
        // default to wait 2 seconds between each request
        public static FuzzyInt MILLI_BETWEEN_EXEC = new FuzzyInt(2000);

        // make sure our first request doesn't wait
        private static DateTime m_LAST_EXEC = DateTime.Now.AddMilliseconds(-MILLI_BETWEEN_EXEC.Maximum);

        protected abstract string APIPath { get; }

        protected virtual Dictionary<string, string> APIParameters
        {
            get { return new Dictionary<string, string>(); }
        }

        protected virtual Method APIMethod
        {
            get { return Method.GET; }
        }

        protected virtual bool RequiresGameSession
        {
            get { return false; }
        }

        public T Execute()
        {
            RequestUtil request = RequestUtil.NewInstance(Globals.TRADINGPOST_HOST, APIPath);

            request.SetMethod(APIMethod);

            foreach (KeyValuePair<string, string> parameter in APIParameters)
                request.SetParameter(parameter.Key, parameter.Value);

            SessionManager sm = SessionManager.GetInstance();
            Session s = (RequiresGameSession ? sm.GameSession : sm.Session);

            request.SetCookie("s", s.Key);
            
            T response = null;

            // don't let simultaneous requests happen
            lock (this)
            {
                TimeSpan span = DateTime.Now - m_LAST_EXEC;
                int wait = MILLI_BETWEEN_EXEC.Value;
                if (span.TotalMilliseconds < wait)
                    Thread.Sleep(wait - (int)span.TotalMilliseconds);

                response = request.Execute<T>();

                m_LAST_EXEC = DateTime.Now;
            }

            return response;
        }
    }
}
