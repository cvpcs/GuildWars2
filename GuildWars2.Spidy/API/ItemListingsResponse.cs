using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp.Deserializers;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class ItemListingsResponse : PaginatedResponse
    {
        [DeserializeAs(Name="sell-or-buy")]
        public string SellOrBuy { get; set; }

        public int Total { get; set; }

        public List<ItemListing> Results { get; set; }
    }
}
