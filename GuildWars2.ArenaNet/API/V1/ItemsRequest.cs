using System;

namespace GuildWars2.ArenaNet.API.V1
{
    public class ItemsRequest : Request<ItemsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/items.json"; }
        }
    }
}
