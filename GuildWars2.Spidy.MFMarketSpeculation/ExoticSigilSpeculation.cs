using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.MFMarketSpeculation
{
    public class ExoticRuneSpeculation : BaseSpeculation
    {
        public override string Name { get { return "Exotic Sigils"; } }

        public override void Run()
        {
            // get data
            ItemType typeUC = GetItemType("Upgrade Component");
            IList<ItemData> upgrade_components = new FullItemListRequest(typeUC)
                    .Execute()
                    .Results
                    .ToList();
            IList<ItemData> major_sigils = upgrade_components
                    .Where(i => i.Name.StartsWith("Major Sigil of"))
                    .ToList();
            IList<ItemData> superior_sigils = upgrade_components
                    .Where(i => i.Name.StartsWith("Superior Sigil of"))
                    .ToList();

            Console.WriteLine("Gathering Data:");
            Console.Write("  Capital [10 00 00]: ");
            string sCapital = Console.ReadLine();

            int capital;
            if (!int.TryParse(sCapital.Replace(" ", string.Empty).Trim(), out capital))
                capital = 100000;

            Console.WriteLine("Calculating using the following:");
            Console.WriteLine("  Capital: {0}", capital.ToString("#### ## #0").Trim());

            int buy_price = major_sigils.Select<ItemData, int>(i => i.MaxOfferUnitPrice).Min();
            int sell_price = (int)Math.Floor(superior_sigils.Select<ItemData, int>(i => i.MinSaleUnitPrice).Average());
            int total_available = (int)Math.Floor((double)capital / (double)buy_price);
            int total_cost = buy_price * total_available;

            Console.WriteLine("Rare sigil buy price (based on cheapest rare sigils): {0}", buy_price.ToString("#### ## #0").Trim());
            Console.WriteLine("Exotic sigil sell price (based on average exotic sigils): {0}", sell_price.ToString("#### ## #0").Trim());
            Console.WriteLine("Total sigils purchased: {0}", total_available);
            Console.WriteLine("Total cost: {0}", total_cost.ToString("#### ## #0").Trim());

            double drop_rate = 0.2;

            // mystic forge loop
            int total_sellables = 0;
            int total_forges = 0;
            while (total_available >= 4)
            {
                // how many sigils we get back
                int returns = total_available / 4;

                // remainder
                total_available %= 4;

                // how many sellables we should get
                int sellables = (int)Math.Floor((double)returns * drop_rate);

                total_available += (returns - sellables);

                total_sellables += sellables;
                total_forges += returns;
            }

            Console.WriteLine("Estimated forges: {0}", total_forges);
            Console.WriteLine("Estimated sellables: {0}", total_sellables);
            
            int total_revenue = (int)Math.Floor(total_sellables * sell_price * 0.85);
            int total_profit = total_revenue - total_cost;

            Console.WriteLine("Estimates:");
            Console.WriteLine("  Revenue: {0}", total_revenue.ToString("#### ## #0").Trim());
            Console.WriteLine("  Profit: {0}", total_profit.ToString("#### ## #0").Trim());
        }
    }
}