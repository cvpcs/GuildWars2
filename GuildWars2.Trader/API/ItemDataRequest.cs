using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Trader.Model;

namespace GuildWars2.Trader.API
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
            return "/api/public/item";
        }

        protected override Dictionary<string, string> GetAPIParameters()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["id"] = DataId.ToString();
            return param;
        }
    }
}
