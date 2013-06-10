using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class ItemListingsRequest : PaginatedRequest<ItemListingsResponse>
    {
        public int DataId { get; set; }
        public ListingType SellOrBuy { get; set; }

        public ItemListingsRequest(int dataId, ListingType sellOrBuy, int page = 1)
            : base(page)
        {
            DataId = dataId;
            SellOrBuy = sellOrBuy;
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/listings/" + DataId + "/" + (SellOrBuy == ListingType.SELL ? "sell" : "buy") + "/" + Page;
        }

        public enum ListingType { SELL, BUY }
    }
}
