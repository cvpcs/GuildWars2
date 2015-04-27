using System;

namespace GuildWars2.ArenaNet.API.V2.Commerce
{
    public class PricesRequest : Request<int, PricesResponse>
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
