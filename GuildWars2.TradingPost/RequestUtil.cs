using System;
using System.Collections.Generic;
using System.Net;

using RestSharp;

namespace GuildWars2.TradingPost
{
    public class RequestUtil
    {
        private RestClient m_Client;
        private RestRequest m_Request;

        public static RequestUtil NewInstance(string host, string path)
        {
            return new RequestUtil(host, path);
        }

        private RequestUtil(string host, string path)
        {
            m_Client = new RestClient(host);
            m_Client.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:13.0) Gecko/20100101 Firefox/13.0.1";
            m_Client.Timeout = 10000; // 10 second timeout
            m_Client.FollowRedirects = true;
            m_Client.MaxRedirects = 5;
            m_Client.CookieContainer = new CookieContainer(10, 5, 256);

            // default headers
            m_Client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            m_Client.AddDefaultHeader("Accept-Charset", "en");
            m_Client.AddDefaultHeader("Accept-Encoding", "deflate");
            m_Client.AddDefaultHeader("Accept-Language", "en;q=0.5");
            m_Client.AddDefaultHeader("Referer", host + path);

            m_Request = new RestRequest();
            m_Request.Resource = path;
        }

        public RequestUtil SetHeader(string name, string value)
        {
            m_Request.AddHeader(name, value);
            return this;
        }

        public RequestUtil SetCookie(string name, string value)
        {
            m_Request.AddCookie(name, value);
            return this;
        }

        public RequestUtil SetMethod(Method method)
        {
            m_Request.Method = method;
            return this;
        }

        public RequestUtil SetParameter(string name, string value)
        {
            
            m_Request.AddParameter(name, value);
            return this;
        }

        public RequestUtil SetFollowRedirects(bool value)
        {
            m_Client.FollowRedirects = value;
            return this;
        }

        public IRestResponse Execute()
        {
            return m_Client.Execute(m_Request);
        }

        public T Execute<T>()
            where T : class, new()
        {
            IRestResponse<T> response = m_Client.Execute<T>(m_Request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }
    }
}
