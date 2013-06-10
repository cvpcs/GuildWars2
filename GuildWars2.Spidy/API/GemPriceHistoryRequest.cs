using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Spidy.API
{
    public class GemPriceHistoryRequest : PaginatedRequest<GemPriceHistoryResponse>
    {
        public GemPriceHistoryRequest(int page = 1)
            : base(page)
        {
            throw new NotImplementedException("Guild Wars 2 Spidy has not implemented this request yet.");
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/gem-history/" + Page;
        }
    }
}
