using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.TradingPost.Model;

namespace GuildWars2.TradingPost.API
{
    public class MeResponse : CountedResponse
    {
        public List<Listing> Listings { get; set; }
    }
}
