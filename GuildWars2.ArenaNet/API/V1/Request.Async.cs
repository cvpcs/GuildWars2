using System;
using System.Collections.Generic;

using RestSharp;

namespace GuildWars2.ArenaNet.API.V1
{
    public abstract partial class Request<T>
        where T : class, new()
    {
        public virtual void ExecuteAsync(Action<T> callback)
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
                request.AddParameter(parameter.Key, parameter.Value);
            }

            client.ExecuteAsync<T>(request, response =>
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        callback(response.Data);
                    else
                        callback(null);
                });
        }
    }
}
