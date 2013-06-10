using System;
using System.Collections.Generic;

using RestSharp;

namespace GuildWars2.TradingPost.API
{
    public class BuyRequest : Request<BuyResponse>
    {
        public int DataId { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string CharId { get; set; }

        protected override string APIPath
        {
            get { return string.Format("/ws/item/{0}/buy", DataId); }
        }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["count"] = Count.ToString();
                parameters["price"] = Price.ToString();
                parameters["charid"] = CharId;

                return parameters;
            }
        }

        protected override RestSharp.Method APIMethod
        {
            get { return Method.POST; }
        }

        protected override bool RequiresGameSession
        {
            get { return true; }
        }

        public BuyRequest(int data_id, int count, int price, string charId)
        {
            DataId = data_id;
            Count = count;
            Price = price;
            CharId = charId;
        }
    }
}
