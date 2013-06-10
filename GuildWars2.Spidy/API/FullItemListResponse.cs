using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class FullItemListResponse : CountedResponse
    {
        public List<ItemData> Results { get; set; }
    }
}
