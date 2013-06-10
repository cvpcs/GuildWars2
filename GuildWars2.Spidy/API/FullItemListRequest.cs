using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class FullItemListRequest : Request<FullItemListResponse>
    {
        public ItemType Type { get; set; }

        public FullItemListRequest(ItemType type = null)
        {
            Type = type;
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/all-items/" + (Type == null ? "*all*" : Type.Id.ToString());
        }
    }
}
