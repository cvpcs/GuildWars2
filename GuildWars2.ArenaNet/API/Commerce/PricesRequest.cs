using System;

namespace GuildWars2.ArenaNet.API.Commerce
{
    public class PricesRequest : RequestV2<int, PricesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/commerce/prices"; }
        }

        public PricesRequest(params int[] item_ids)
            : base(item_ids)
        { }
    }
}
