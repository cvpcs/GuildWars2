using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class ItemSearchRequest : PaginatedRequest<ItemSearchResponse>
    {
        public string Name { get; set; }

        public ItemSearchRequest(string name, int page = 1)
            : base(page)
        {
            Name = name;
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/item-search/" + Name + "/" + Page;
        }
    }
}
