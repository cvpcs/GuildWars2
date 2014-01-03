using System;
using System.Collections.Generic;

using RestSharp;

namespace GuildWars2.GoMGoDS.API
{
    public abstract partial class Request<T>
        where T : class, new()
    {
        public T Execute()
        {
            RestClient client = new RestClient();
            client.BaseUrl = URL;
            client.Timeout = Timeout;

            // use custom JSON deserializer
            client.AddHandler("application/json", new RestSharpDataContractJsonDeserializer());

            RestRequest request = new RestRequest();
            request.Method = APIMethod;
            request.Resource = APIPath;

            foreach (KeyValuePair<string, string> parameter in APIParameters)
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
