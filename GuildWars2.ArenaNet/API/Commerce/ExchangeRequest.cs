using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.API.Commerce
{
    public class ExchangeRequest : RequestV2<string, ExchangeResponse>
    {
        public int Quantity { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;
                
                // remove ids if they exist, ids for exchange are appended to the api path
                if (parameters.ContainsKey("id"))
                    parameters.Remove("id");
                if (parameters.ContainsKey("ids"))
                    parameters.Remove("ids");

                if (Id.Length > 0)
                    parameters["quantity"] = Quantity.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/commerce/exchange" + (Id.Length > 0 ? "/" + Id : string.Empty); }
        }

        public ExchangeRequest()
            : base()
        { }

        public ExchangeRequest(string id, int quantity)
            : base(id)
        {
            Quantity = quantity;
        }
    }
}
