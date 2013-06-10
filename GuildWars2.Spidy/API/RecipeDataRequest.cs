using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class RecipeDataRequest : Request<RecipeDataResponse>
    {
        public int DataId { get; set; }

        public RecipeDataRequest(int dataId)
        {
            DataId = dataId;
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/recipe/" + DataId;
        }
    }
}
