using System;

namespace GuildWars2.TradingPost.Model
{
    public class Listing : Item
    {
        public DateTime Created { get; set; }

        public string Fuzzy { get; set; }

        public long ListingId { get; set; }

        public DateTime? Purchased { get; set; }

        public int Quantity { get; set; }

        public int UnitPrice { get; set; }
    }
}
