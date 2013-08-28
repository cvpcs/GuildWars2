using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

using GuildWars2.Overlay.Contract;
using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.Prospects
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HotKeyHandler hotKeyHandler = new HotKeyHandler();
          
            SearchConfig searchConfig;
            SearchConfig defaultSearchConfig = new SearchConfig(
                    ConfigurationManager.AppSettings["Type"],
                    ConfigurationManager.AppSettings["Rarity"],
                    ConfigurationManager.AppSettings["Minimum Level"],
                    ConfigurationManager.AppSettings["Maximum Level"],
                    ConfigurationManager.AppSettings["Minimum Profit Margin"],
                    ConfigurationManager.AppSettings["Copper Value"]);
            string type, rarity, minLevel, maxLevel, minProfitMargin, copperValue;

            Console.WriteLine("Gathering search data:");
            Console.Write("  Type [{0}]: ",
                    (defaultSearchConfig.Types.Count == 0 ? "*" : string.Join(",", defaultSearchConfig.Types)));
            type = Console.ReadLine();
            Console.Write("  Rarity [{0}]: ",
                    (defaultSearchConfig.Rarities.Count == 0 ? "*" : string.Join(",", defaultSearchConfig.Rarities)));
            rarity = Console.ReadLine();
            Console.Write("  Minimum Level [{0}]: ", defaultSearchConfig.MinimumLevel);
            minLevel = Console.ReadLine();
            Console.Write("  Maximum Level [{0}]: ", defaultSearchConfig.MaximumLevel);
            maxLevel = Console.ReadLine();
            Console.Write("  Minimum Profit Margin [{0}]: ",
                    (defaultSearchConfig.MarginIsPercent ?
                            defaultSearchConfig.MinimumProfitMargin + "%" :
                            defaultSearchConfig.MinimumProfitMargin.ToString("#### ## #0").Trim()));
            minProfitMargin = Console.ReadLine().Replace(" ", string.Empty);
            Console.Write("  Copper Value [{0}]: ", defaultSearchConfig.CopperValue);
            copperValue = Console.ReadLine();

            searchConfig = new SearchConfig(type, rarity, minLevel, maxLevel, minProfitMargin, copperValue, defaultSearchConfig);

            // load overlay handler
            OverlayHandler.Initialize();

            Console.WriteLine("Pulling prospects for the following data:");
            Console.WriteLine("  Type: {0}",
                    (searchConfig.Types.Count == 0 ? "*" : string.Join(",", searchConfig.Types)));
            Console.WriteLine("  Rarity: {0}",
                    (searchConfig.Rarities.Count == 0 ? "*" : string.Join(",", searchConfig.Rarities)));
            Console.WriteLine("  Minimum Level: {0}", searchConfig.MinimumLevel);
            Console.WriteLine("  Maximum Level: {0}", searchConfig.MaximumLevel);
            Console.WriteLine("  Minimum Profit Margin: {0}", 
                    (searchConfig.MarginIsPercent ?
                            searchConfig.MinimumProfitMargin + "%" :
                            searchConfig.MinimumProfitMargin.ToString("#### ## #0").Trim()));
            Console.WriteLine("  Copper Value: {0}", searchConfig.CopperValue);
            Console.WriteLine("  Order by: Buy price, ascending");

            Console.Write("Grabbing prospects: ");
            IList<ItemInfo> prospects = new List<ItemInfo>();

            try
            {
                prospects = fetchProspects(searchConfig);
                ok();
            }
            catch (Exception)
            {
                error();
                Console.WriteLine("  There was a problem communicating with Gw2Spidy. The website may be offline.");
            }

            if (prospects.Count > 0)
            {
                Console.WriteLine("Found {0} prospects. Press any key to view the next result (Esc to exit):", prospects.Count);
                Console.WriteLine("{0} {1,-10} {2,-10} [{3,-10}] {4}", " ", "Sell", "Buy", "Margin", "Name");

                hotKeyHandler.CopperValue = searchConfig.CopperValue;
                hotKeyHandler.RegisterHotKeys();

                foreach (ItemInfo item in prospects)
                {
                    Console.WriteLine(string.Format("{0} {1,10} {2,10} [{3,10}] {4}",
                            ((searchConfig.CopperValue == (int)(item.BuyPrice % 100) || searchConfig.CopperValue == (int)(item.SellPrice)) ? "*" : " "),
                            item.SellPrice.ToString("#### ## #0").Trim(),
                            item.BuyPrice.ToString("#### ## #0").Trim(),
                            item.ProfitMargin.ToString("#### ## #0").Trim(),
                            item.Name));

                    hotKeyHandler.CurrentItem = item;

                    // load item into overlay
                    OverlayHandler.LoadItem(
                            item, 
                            (searchConfig.CopperValue == (int)(item.BuyPrice % 100) ? 1 : 
                            (searchConfig.CopperValue == (int)(item.SellPrice % 100) ? 3  : 0)));

                    SpinWait.SpinUntil(() => Console.KeyAvailable || hotKeyHandler.CurrentItem == null);

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                            break;
                    }
                }

                hotKeyHandler.UnregisterHotKeys();
            }
            else
            {
                Console.WriteLine("Found 0 prospects. Press any key to exit.");
                Console.ReadKey(true);
            }
        }

        private static void error()
        {
            Console.WriteLine("[!!]");
        }

        private static void ok()
        {
            Console.WriteLine("[ok]");
        }

        private static IList<ItemInfo> fetchProspects(SearchConfig search)
        {
            // get rarities
            IList<Rarity> rarities = new RarityListRequest().Execute().Results;

            // get types
            IList<ItemType> types = new TypeListRequest().Execute().Results;

            // get all armors that i want to sell
            IList<ItemInfo> items = new FullItemListRequest().Execute()
                    .Results
                    .Where(i =>
                        (search.Types.Count == 0 ?
                                true :
                                types.Where(t => search.Types.Contains(t.Name)).Select(t => t.Id).Contains(i.TypeId)))
                    .Where(i =>
                        (search.Rarities.Count == 0 ?
                                true :
                                rarities.Where(r => search.Rarities.Contains(r.Name)).Select(r => r.Id).Contains(i.Rarity)))
                    .Where(i =>
                        (search.MaximumLevel < 0 ?
                                true :
                                i.RestrictionLevel <= search.MaximumLevel) &&
                        (search.MinimumLevel < 0 ?
                                true :
                                i.RestrictionLevel >= search.MinimumLevel))
                    .Where(i => i.OfferAvailability > 0 && i.SaleAvailability > 0)
                    .Select<ItemData, ItemInfo>(a => ItemInfo.FromItemData(a, rarities, types))
                    .Where(a => 
                        (search.MinimumProfitMargin < 0 ?
                                true :
                                (search.MarginIsPercent ? a.ProfitMargin >= a.BuyPrice * (search.MinimumProfitMargin / 100.0) : a.ProfitMargin >= search.MinimumProfitMargin)))
                    .OrderBy(a => a.BuyPrice)
                    .ToList();

            return items;
        }

        private class SearchConfig
        {
            public IList<string> Types;
            public IList<string> Rarities;
            public int MinimumLevel;
            public int MaximumLevel;
            public int MinimumProfitMargin;
            public bool MarginIsPercent;
            public int CopperValue;

            public SearchConfig(string types, string rarities, string minLevel, string maxLevel, string minProfitMargin, string copperValue, SearchConfig defaults = null)
            {
                if (string.IsNullOrWhiteSpace(types))
                    this.Types = (defaults == null ? new List<string>() : defaults.Types);
                else
                {
                    this.Types = new List<string>();

                    if (types != "*")
                    {
                        foreach (string type in types.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(type))
                                this.Types.Add(type.Trim());
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(rarities))
                    this.Rarities = (defaults == null ? new List<string>() : defaults.Rarities);
                else
                {
                    this.Rarities = new List<string>();

                    if (rarities != "*")
                    {
                        foreach (string rarity in rarities.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(rarity))
                                this.Rarities.Add(rarity.Trim());
                        }
                    }
                }

                if (!int.TryParse(minLevel, out this.MinimumLevel))
                    this.MinimumLevel = (defaults == null ? 0 : defaults.MinimumLevel);

                if (!int.TryParse(maxLevel, out this.MaximumLevel))
                    this.MaximumLevel = (defaults == null ? 80 : defaults.MaximumLevel);

                this.MarginIsPercent = false;
                if (!int.TryParse(minProfitMargin, out this.MinimumProfitMargin))
                {
                    this.MinimumProfitMargin = (defaults == null ? -1 : defaults.MinimumProfitMargin);

                    if (minProfitMargin.EndsWith("%"))
                    {
                        this.MarginIsPercent = true;
                        if (!int.TryParse(minProfitMargin.Substring(0, minProfitMargin.Length - 1), out this.MinimumProfitMargin))
                        {
                            this.MinimumProfitMargin = (defaults == null ? -1 : defaults.MinimumProfitMargin);
                        }
                    }
                }

                if (!int.TryParse(copperValue, out this.CopperValue))
                    this.CopperValue = (defaults == null ? -1 : defaults.CopperValue);
            }
        }
    }
}
