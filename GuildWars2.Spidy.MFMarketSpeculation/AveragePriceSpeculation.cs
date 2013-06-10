using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.MFMarketSpeculation
{
    public class AveragePriceSpeculation : BaseSpeculation
    {
        public override string Name { get { return "Average Price"; } }

        public override void Run()
        {
            string sMinLevel, sMaxLevel;
            int minLevel, maxLevel;
            Rarity rarity;

            Console.WriteLine("Gathering search data:");
            Console.Write("  Rarity [Exotic]: ");
            rarity = GetRarity(Console.ReadLine());
            Console.Write("  Minimum Level [80]: ");
            sMinLevel = Console.ReadLine();
            Console.Write("  Maximum Level [80]: ");
            sMaxLevel = Console.ReadLine();

            if (rarity.Id < 0)
                rarity = GetRarity("Exotic");
            if (!int.TryParse(sMinLevel, out minLevel))
                minLevel = 80;
            if (!int.TryParse(sMaxLevel, out maxLevel))
                maxLevel = 80;

            Console.WriteLine("Pulling averages for the following data:");
            Console.WriteLine("  Rarity: {0}", rarity.Name);
            Console.WriteLine("  Minimum Level: {0}", minLevel);
            Console.WriteLine("  Maximum Level: {0}", maxLevel);

            IList<ItemData> all_items = new FullItemListRequest().Execute().Results
                    .Where(i => i.RestrictionLevel <= maxLevel && i.RestrictionLevel >= minLevel)
                    .Where(i => i.Rarity == rarity.Id)
                    .Where(i => i.OfferAvailability > 0 && i.SaleAvailability > 0)
                    .ToList();

            Console.WriteLine("Averages:");
            Console.WriteLine("{0,-10} | {1,-10} | {2}", "Avg Buy", "Avg Sell", "Type");
            Console.WriteLine("-----------+------------+-----------------------------");

            foreach (ItemType type in Types)
            {
                IList<ItemData> items = all_items.Where(a => a.TypeId == type.Id).ToList();

                if (items.Count == 0)
                    continue;

                int avg_buy = (int)Math.Floor(items.Select<ItemData, int>(i => i.MaxOfferUnitPrice).InterquartileMean());
                int avg_sell = (int)Math.Floor(items.Select<ItemData, int>(i => i.MinSaleUnitPrice).InterquartileMean());
                Console.WriteLine("{0,10} | {1,10} | {2}",
                        avg_buy.ToString("#### ## #0").Trim(),
                        avg_sell.ToString("#### ## #0").Trim(),
                        type.Name);

                foreach (ItemSubType sub_type in type.SubTypes)
                {
                    IList<ItemData> sub_items = items.Where(a => a.SubTypeId == sub_type.Id).ToList();

                    if (sub_items.Count == 0)
                        continue;

                    int sub_avg_buy = (int)Math.Floor(sub_items.Select<ItemData, int>(i => i.MaxOfferUnitPrice).InterquartileMean());
                    int sub_avg_sell = (int)Math.Floor(sub_items.Select<ItemData, int>(i => i.MinSaleUnitPrice).InterquartileMean());
                    Console.WriteLine("{0,10} | {1,10} | {2}",
                            sub_avg_buy.ToString("#### ## #0").Trim(),
                            sub_avg_sell.ToString("#### ## #0").Trim(),
                            sub_type.Name);
                }

                Console.WriteLine("-----------+------------+-----------------------------");
            }
        }
    }
}