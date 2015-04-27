using System;

namespace GuildWars2.ArenaNet.API.V2.Commerce
{
    public class ListingsRequest : Request<int, ListingsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/commerce/listings"; }
        }

        public ListingsRequest(params int[] item_ids)
            : base(item_ids)
        { }
    }
}
