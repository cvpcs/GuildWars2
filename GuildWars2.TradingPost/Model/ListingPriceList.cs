using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.Model
{
    public class ListingPriceList
    {
        public int DataId { get; set; }

        public List<ListingPrice> Buys { get; set; }

        public List<ListingPrice> Sells { get; set; }
    }
}
