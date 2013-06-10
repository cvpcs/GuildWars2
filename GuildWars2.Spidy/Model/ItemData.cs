using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Spidy.Model
{
    public class ItemData
    {
        public int DataId { get; set; }

        public string Name { get; set; }

        public int Rarity { get; set; }

        public int RestrictionLevel { get; set; }

        public string Img { get; set; }

        public int TypeId { get; set; }

        public int SubTypeId { get; set; }

        public DateTime PriceLastChanged { get; set; }

        public int MaxOfferUnitPrice { get; set; }

        public int MinSaleUnitPrice { get; set; }

        public int Gw2dbExternalId { get; set; }

        public int SalePriceChangeLastHour { get; set; }

        public int OfferPriceChangeLastHour { get; set; }

        public int SaleAvailability { get; set; }

        public int OfferAvailability { get; set; }
    }
}
