using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.MFMarketSpeculation
{
    public class ArmorWeaponSpeculation : BaseSpeculation
    {
        public override string Name { get { return "Armor / Weapons"; } }

        public override void Run()
        {
            // get data
            ItemType typeA = GetItemType("Armor");
            ItemType typeW = GetItemType("Weapon");
            IList<ItemData> armor = new FullItemListRequest(typeA).Execute().Results;
            IList<ItemData> weapons = new FullItemListRequest(typeW).Execute().Results;

            Console.WriteLine("Gathering Data:");
            Console.Write("  Capital [10 00 00]: ");
            string sCapital = Console.ReadLine();
            Console.Write("  Maximum buy price [2 00]: ");
            string sMaxBuyPrice = Console.ReadLine();
            Console.Write("  Buy rarity [Masterwork]: ");
            Rarity rarityB = GetRarity(Console.ReadLine());

            int capital, max_buy_price;
            if (!int.TryParse(sCapital.Replace(" ", string.Empty).Trim(), out capital))
                capital = 100000;
            if (!int.TryParse(sMaxBuyPrice.Replace(" ", string.Empty).Trim(), out max_buy_price))
                max_buy_price = 200;
            if (rarityB.Id < 0)
                rarityB = GetRarity("Masterwork");
            Rarity rarityS = GetPromotedRarity(rarityB);

            Console.WriteLine("Calculating using the following:");
            Console.WriteLine("  Capital: {0}", capital.ToString("#### ## #0").Trim());
            Console.WriteLine("  Maximum buy price: {0}", max_buy_price.ToString("#### ## #0").Trim());
            Console.WriteLine("  Buy rarity: {0}", rarityB.Name);
            Console.WriteLine("  Sell rarity: {0}", rarityS.Name);

            IDictionary<int, int> buy_level_profits = new Dictionary<int, int>();

            for (int min_buy_level = (68 - 5); min_buy_level <= 68; min_buy_level++)
            {
                Console.WriteLine("Performing speculation on minimum buy level: {0}", min_buy_level);

                int min_forge_level = min_buy_level + 5;
                int max_forge_level = min_buy_level + 12;

                Console.WriteLine("  Forged item range: {0} - {1}", min_forge_level, max_forge_level);

                IList<ItemData> buy_weapons = weapons
                        .Where(i => i.Rarity == rarityB.Id && i.MinSaleUnitPrice > 0 && i.RestrictionLevel >= min_buy_level)
                        .ToList();
                IList<ItemData> sell_weapons = weapons
                        .Where(i => i.Rarity == rarityS.Id && i.MinSaleUnitPrice > 0 && i.RestrictionLevel >= min_forge_level && i.RestrictionLevel <= max_forge_level)
                        .ToList();

                int buy_price = (int)Math.Floor((double)(buy_weapons.Select<ItemData, int>(i => i.MinSaleUnitPrice).Min() + max_buy_price)/2.0);
                int sell_price = (int)Math.Floor(sell_weapons.Select<ItemData, int>(i => i.MinSaleUnitPrice).InterquartileMean());

                int total_available = (int)Math.Floor((double)capital / (double)buy_price);
                int total_cost = buy_price * total_available;

                Console.WriteLine("  {0} Weapon buy price (based on average of cheapest and maximum buy price): {1}", rarityB.Name, buy_price.ToString("#### ## #0").Trim());
                Console.WriteLine("  {0} Weapon sell price (based on IQM of current sale prices): {1}", rarityS.Name, sell_price.ToString("#### ## #0").Trim());

                Console.WriteLine("  Total weapons purchased: {0}", total_available);
                Console.WriteLine("  Total cost: {0}", total_cost.ToString("#### ## #0").Trim());

                // mystic forge loop
                int total_rares = 0;
                int total_forges = 0;
                while (total_available >= 4)
                {
                    // how many weapons we get back
                    int returns = total_available / 4;

                    // remainder
                    total_available %= 4;

                    // how many sellables we should get
                    int upgrades = (int)Math.Floor((double)returns * PromotionRate);

                    total_available += (returns - upgrades);

                    total_rares += upgrades;
                    total_forges += returns;
                }

                Console.WriteLine("  Estimated forges: {0}", total_forges);
                Console.WriteLine("  Estimated rares: {0}", total_rares);

                int total_rare_revenue = (int)Math.Floor(total_rares * sell_price * 0.85);
                int total_profit = total_rare_revenue - total_cost;

                Console.WriteLine("  Estimated Revenue: {0}", total_rare_revenue.ToString("#### ## #0").Trim());
                Console.WriteLine("  Estimated Profit: {0}", total_profit.ToString("#### ## #0").Trim());

                buy_level_profits[total_profit] = min_buy_level;
            }

            int suggested_min_buy_level = buy_level_profits[buy_level_profits.Keys.Max()];

            Console.WriteLine("Suggested minimum buy level: {0}", suggested_min_buy_level);
        }
    }
}