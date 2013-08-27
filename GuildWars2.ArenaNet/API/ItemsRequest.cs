using System;

namespace GuildWars2.ArenaNet.API
{
    public class ItemsRequest : Request<ItemsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/items.json"; }
        }
    }
}
