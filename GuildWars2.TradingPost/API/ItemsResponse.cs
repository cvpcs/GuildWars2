using System;
using System.Collections.Generic;

using GuildWars2.TradingPost.Model;

namespace GuildWars2.TradingPost.API
{
    public class ItemsResponse : CountedResponse
    {
        public List<Item> Results { get; set; }
    }
}
