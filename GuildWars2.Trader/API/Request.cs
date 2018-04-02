using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;

namespace GuildWars2.Trader.API
{
    public abstract class Request<T>
        where T : class, new()
    {
        public static int Timeout = 10000;

        private static Uri URL = new Uri("http://www.guildwarstrade.com/");

        protected abstract string GetAPIPath();
        protected virtual Dictionary<string, string> GetAPIParameters()
        {
            return new Dictionary<string, string>();
        }

        public T Execute()
        {
            RestClient client = new RestClient();
            client.BaseUrl = URL;
            client.Timeout = Timeout;
            client.AddHandler("text/html", new RestSharp.Deserializers.JsonDeserializer());

            RestRequest request = new RestRequest();
            request.Method = Method.GET;
            request.Resource = GetAPIPath();

            foreach (KeyValuePair<string, string> parameter in GetAPIParameters())
            {
                request.AddParameter(parameter.Key, parameter.Value);
            }

            IRestResponse<T> response = client.Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }
    }
}
