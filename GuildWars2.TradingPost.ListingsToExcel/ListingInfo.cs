using System;

using GuildWars2.TradingPost.API;
using GuildWars2.TradingPost.Model;

namespace GuildWars2.TradingPost.ListingsToExcel
{
    public class ListingInfo
    {
        public const double LISTING_FEE_PERCENT = 0.05;
        public const double SALE_FEE_PERCENT = 0.10;

        public long ListingId { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemLevel { get; set; }
        public string ItemRarity { get; set; }
        public long Quantity { get; set; }
        public long Price { get; set; }
        public DateTime ListingTime { get; set; }
        public DateTime? FulfilledTime { get; set; }
        public ListingType Type { get; set; }
        public bool Cancelled { get; set; }

        public bool Fulfilled { get { return FulfilledTime != null; } }

        public long LiquidInvestment
        {
            get
            {
                if (Type == ListingType.BUY && !Fulfilled && !Cancelled)
                    return Quantity * Price; // if we haven't fulfilled or cancelled our buy order yet, we can get our money back
                else
                    return 0;
            }
        }

        public long SolidInvestment
        {
            get
            {
                // if it's a sell order, our solid investment is always going to be the listing fee
                if (Type == ListingType.SELL)
                    return (long)Math.Ceiling(Quantity * Price * LISTING_FEE_PERCENT); // solid investment is always the listing fee in a sale
                else if (Type == ListingType.BUY && Fulfilled)
                    return Quantity * Price; // solid investment is the cost of purchase on any fulfilled buy orders
                else
                    return 0;
            }
        }

        public long Investment { get { return LiquidInvestment + SolidInvestment; } }


        public long UnrealizedRevenue
        {
            get
            {
                if (Type == ListingType.SELL && !Fulfilled && !Cancelled)
                    return (long)Math.Floor(Quantity * Price * (1.0 - SALE_FEE_PERCENT));
                else
                    return 0;
            }
        }

        public long Revenue
        {
            get
            {
                if (Type == ListingType.SELL && Fulfilled)
                    return (long)Math.Floor(Quantity * Price * (1.0 - SALE_FEE_PERCENT));
                else
                    return 0;
            }
        }

        public enum ListingType { BUY, SELL }

        public static ListingInfo FromListing(Listing listing, MeRequest.ListingType type)
        {
            ListingInfo info = new ListingInfo();

            switch (type)
            {
                case MeRequest.ListingType.BUY:
                    info.Type = ListingInfo.ListingType.BUY;
                    break;
                case MeRequest.ListingType.SELL:
                    info.Type = ListingInfo.ListingType.SELL;
                    break;
                default:
                    break;
            }

            info.ListingId = listing.ListingId;
            info.ItemId = listing.DataId;
            info.ItemName = listing.Name;
            info.ItemLevel = listing.Level;
            info.ItemRarity = SpidyData.GetRarityNameFromId(listing.Rarity);
            info.Quantity = listing.Quantity;
            info.Price = listing.UnitPrice;
            info.ListingTime = listing.Created;
            info.FulfilledTime = listing.Purchased;

            return info;
        }
    }
}
