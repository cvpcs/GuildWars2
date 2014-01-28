using System;
using System.Collections.Generic;

using RestSharp;

using GuildWars2.ArenaNet;

namespace GuildWars2.GoMGoDS.API
{
    public abstract partial class Request<T>
        where T : class, new()
    {
        public void ExecuteAsync(Action<T> callback)
        {
            RestClient client = new RestClient();
            client.BaseUrl = URL;
            client.Timeout = Timeout;

            // use custom JSON deserializer
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
