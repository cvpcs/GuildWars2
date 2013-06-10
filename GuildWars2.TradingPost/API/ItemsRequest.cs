using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.API
{
    public class ItemsRequest : CountedRequest<ItemsResponse>
    {
        public IList<int> DataIds { get; set; }

        protected override string APIPath
        {
            get { return "/ws/items.json"; }
        }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["ids"] = string.Join<int>(",", DataIds);

                return parameters;
            }
        }

        public ItemsRequest(params int[] ids)
        {
            DataIds = new List<int>();

            foreach (int id in ids)
                DataIds.Add(id);
        }
    }
}
