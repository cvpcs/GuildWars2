using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Spidy.API
{
    public class ItemDataRequest : Request<ItemDataResponse>
    {
        public int DataId { get; set; }

        public ItemDataRequest(int dataId)
        {
            DataId = dataId;
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/item/" + DataId;
        }
    }
}
