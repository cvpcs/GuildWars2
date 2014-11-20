using System;

namespace GuildWars2.ArenaNet.API
{
    public class ItemsRequestV2 : RequestV2<int, ItemsResponseV2>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/items"; }
        }

        public ItemsRequestV2(params int[] item_ids)
            : base(item_ids)
        { }
    }
}
