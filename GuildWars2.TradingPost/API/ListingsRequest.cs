using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.API
{
    public class ListingsRequest : Request<ListingsResponse>
    {
        public int DataId { get; set; }
        public ListingType Type { get; set; }

        protected override string APIPath
        {
            get { return "/ws/listings.json"; }
        }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["id"] = DataId.ToString();

                switch (Type)
                {
                    case ListingType.SELL:
                        parameters["type"] = "sell";
                        break;
                    case ListingType.BUY:
                        parameters["type"] = "buy";
                        break;
                    default:
                        break;
                }

                return parameters;
            }
        }

        public ListingsRequest(int id)
        {
            DataId = id;
        }

        public enum ListingType { BUY, SELL }
    }
}
