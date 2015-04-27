using System;

namespace GuildWars2.ArenaNet.API.V2
{
    public class ItemsRequest : Request<int, ItemsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/items"; }
        }

        public ItemsRequest(params int[] item_ids)
            : base(item_ids)
        { }
    }
}
