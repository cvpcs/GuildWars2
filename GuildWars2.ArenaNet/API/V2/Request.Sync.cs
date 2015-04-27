using System;
using System.Collections.Generic;

using RestSharp;

using GuildWars2.ArenaNet;

namespace GuildWars2.ArenaNet.API.V2
{
    public abstract partial class Request<TRID, TRDR> : V1.Request<TRDR>
        where TRDR : class, new()
    {
        public override TRDR Execute()
        {
            if (m_Ids.Count == 0)
                return null;
            else
                return base.Execute();
        }

        public virtual List<TRID> ExecuteIdList()
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

            IRestResponse<List<TRID>> response = client.Execute<List<TRID>>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }
    }
}
