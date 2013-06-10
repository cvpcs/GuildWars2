using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class ItemListRequest : PaginatedRequest<ItemListResponse>
    {
        public ItemType Type { get; set; }
        public TrendingSortMethod SortTrending { get; set; }
        public List<int> FilterIds { get; private set; }

        public ItemListRequest(ItemType type = null, int page = 1)
            : base(page)
        {
            Type = type;
            SortTrending = TrendingSortMethod.NONE;
            FilterIds = new List<int>();
        }

        public ItemListRequest(int page, ItemType type = null)
            : this(type, page)
        {
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/items/" + (Type == null ? "*all*" : Type.Id.ToString()) + "/" + Page;
        }

        protected override Dictionary<string, string> GetAPIParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            switch (SortTrending)
            {
                case TrendingSortMethod.SALE:
                    parameters.Add("sort_trending", "sale");
                    break;
                case TrendingSortMethod.OFFER:
                    parameters.Add("sort_trending", "offer");
                    break;
                case TrendingSortMethod.NONE:
                default:
                    break;
            }

            if (FilterIds.Count > 0)
            {
                parameters.Add("", String.Join(",", FilterIds));
            }

            return parameters;
        }

        public enum TrendingSortMethod { NONE, SALE, OFFER }
    }
}
