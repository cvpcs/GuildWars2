using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RestSharp;

namespace GuildWars2.ArenaNet.API
{
    public abstract partial class RequestV2<TRID, TRDR> : Request<TRDR>
        where TRDR : class, new()
    {
        public override void ExecuteAsync(Action<TRDR> callback)
        {
            if (m_Ids.Count == 0)
                Task.Run(() => callback(null));
            else
                base.ExecuteAsync(callback);
        }

        public virtual void ExecuteIdListAsync(Action<List<TRID>> callback)
        {
            RestClient client = new RestClient();
            client.BaseUrl = URL;
            client.Timeout = Timeout;
            client.AddHandler("application/json", new RestSharpNewtonSoftJsonDeserializer());
            client.AddHandler("text/json", new RestSharpNewtonSoftJsonDeserializer());

            RestRequest request = new RestRequest();
            request.Method = APIMethod;
            request.Resource = APIPath;

            foreach (KeyValuePair<string, string> parameter in APIParameters)
            {
                if (parameter.Key != "id" && parameter.Key != "ids")
                    request.AddParameter(parameter.Key, parameter.Value);
            }

            client.ExecuteAsync<List<TRID>>(request, response =>
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        callback(response.Data);
                    else
                        callback(null);
                });
        }
    }
}
