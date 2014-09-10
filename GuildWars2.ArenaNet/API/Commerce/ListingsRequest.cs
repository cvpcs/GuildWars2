using System;

namespace GuildWars2.ArenaNet.API.Commerce
{
    public class ListingsRequest : RequestV2<int, ListingsResponse>
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
