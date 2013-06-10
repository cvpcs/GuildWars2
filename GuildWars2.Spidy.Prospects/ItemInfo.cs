using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.Prospects
{
    public class ItemInfo
    {
        public const double LISTING_FEE_PERCENT = 0.05;
        public const double SALE_FEE_PERCENT = 0.10;

        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public long BuyPrice { get; set; }
        public long SellPrice { get; set; }

        public long ProfitMargin
        {
            get
            {
                return (long)Math.Ceiling(SellPrice * (1 - (LISTING_FEE_PERCENT + SALE_FEE_PERCENT))) - BuyPrice;
            }
        }

        public static ItemInfo FromItemData(ItemData item, IList<Rarity> rarities, IList<ItemType> types)
        {
            ItemInfo info = new ItemInfo();

            info.Id = item.DataId;
            info.Name = item.Name;
            info.Rarity = rarities.Where(r => r.Id == item.Rarity).FirstOrDefault().Name;
            info.Type = types.Where(t => t.Id == item.TypeId).FirstOrDefault().Name;
            info.BuyPrice = item.MaxOfferUnitPrice;
            info.SellPrice = item.MinSaleUnitPrice;

            return info;
        }
    }
}
