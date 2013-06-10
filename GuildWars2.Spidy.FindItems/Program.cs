using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.FindItems
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string sType, sSubtype, sRarity, sLevel, sBuy, sSell, sMargin;

            Interval level = new Interval("[75,inf)");
            Interval buy = new Interval("(-inf,3000)");
            Interval sell = new Interval("(-inf,inf)");
            Interval margin = new Interval("(-inf,inf)");

            Console.WriteLine("Gathering search data:");
            Console.Write("  Type [Weapon/Staff]: ");
            sType = Console.ReadLine();
            Console.Write("  Rarity [Rare]: ");
            sRarity = Console.ReadLine();
            Console.Write("  Level Range [{0}]: ", level.ToString());
            sLevel = Console.ReadLine();
            Console.Write("  Buy Offer Range [{0}]: ", buy.ToString());
            sBuy = Console.ReadLine();
            Console.Write("  Sell Offer Range [{0}]: ", sell.ToString());
            sSell = Console.ReadLine();
            Console.Write("  Margin Range [{0}]: ", margin.ToString());
            sMargin = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(sType))
                sType = "Weapon/Staff";
            if (string.IsNullOrWhiteSpace(sRarity))
                sRarity = "Rare";
            if (!string.IsNullOrWhiteSpace(sLevel))
                level.Parse(sLevel);
            if (!string.IsNullOrWhiteSpace(sBuy))
                buy.Parse(sBuy);
            if (!string.IsNullOrWhiteSpace(sSell))
                sell.Parse(sSell);
            if (!string.IsNullOrWhiteSpace(sMargin))
                margin.Parse(sMargin);

            if (sType == "*")
            {
                sType = null;
                sSubtype = null;
            }
            else if (sType.Contains("/"))
            {
                string[] s = sType.Split('/');
                sType = s[0];
                sSubtype = s[1];
            }
            else
            {
                sSubtype = null;
            }

            if (sRarity == "*")
                sRarity = null;

            // get type
            ItemType type = null;
            if (!string.IsNullOrWhiteSpace(sType))
                 type = new TypeListRequest().Execute()
                        .Results
                        .Where(t => string.Equals(t.Name, sType, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

            // get subtype
            ItemSubType subtype = null;
            if (type != null && !string.IsNullOrWhiteSpace(sSubtype))
                subtype = type.SubTypes
                        .Where(st => string.Equals(st.Name, sSubtype, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

            // get rarity
            Rarity rarity = null;
            if (!string.IsNullOrWhiteSpace(sRarity))
                rarity = new RarityListRequest().Execute()
                        .Results
                        .Where(r => string.Equals(r.Name, sRarity, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

            Console.WriteLine("Finding items for the following data:");
            Console.WriteLine("  Type: {0}", (type == null ? "*" : type.Name));
            Console.WriteLine("  Subtype: {0}", (subtype == null ? "*" : subtype.Name));
            Console.WriteLine("  Rarity: {0}", (rarity == null ? "*" : rarity.Name));
            Console.WriteLine("  Level Range: {0}", level.ToString());
            Console.WriteLine("  Buy Offer Range: {0}", buy.ToString());
            Console.WriteLine("  Sell Offer Range: {0}", sell.ToString());
            Console.WriteLine("  Margin Range: {0}", margin.ToString());
            Console.WriteLine("  Order by: Sell price, ascending");



            // get all armors that i want to sell
            IList<ItemData> items = new FullItemListRequest(type).Execute()
                    .Results
                    .Where(i => subtype == null || i.SubTypeId == subtype.Id)
                    .Where(i => rarity == null || i.Rarity == rarity.Id)
                    .Where(i => level.Contains(i.RestrictionLevel))
                    .Where(i => buy.Contains(i.MaxOfferUnitPrice))
                    .Where(i => sell.Contains(i.MinSaleUnitPrice))
                    .Where(i => margin.Contains((int)(i.MinSaleUnitPrice * 0.85) - i.MaxOfferUnitPrice))
                    .OrderBy(i => i.MinSaleUnitPrice)
                    .ToList();

            if (items.Count > 0)
            {
                Console.WriteLine("Found {0} items. Press any key to view the next result (Esc to exit):", items.Count);
                Console.WriteLine("{0,-10} | {1,-10} | {2,-10} | {3,-5} | {4}", "Sell", "Buy", "Margin", "Level", "Name");

                foreach (ItemData item in items)
                {
                    Console.WriteLine(string.Format("{0,10} | {1,10} | {2,10} | {3,5} | {4}",
                            item.MinSaleUnitPrice.ToString("#### ## #0").Trim(),
                            item.MaxOfferUnitPrice.ToString("#### ## #0").Trim(),
                            ((int)(item.MinSaleUnitPrice * 0.85) - item.MaxOfferUnitPrice).ToString("#### ## #0").Trim(),
                            item.RestrictionLevel,
                            item.Name));

                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                        break;
                }
            }
            else
            {
                Console.WriteLine("Found 0 items. Press any key to exit.");
                Console.ReadKey(true);
            }
        }
    }
}
