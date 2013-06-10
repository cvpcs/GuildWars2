using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using LibMemorySearch;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;
using GuildWars2.TradingPost.API;
using GuildWars2.TradingPost.Exceptions;
using GuildWars2.TradingPost.Model;

namespace GuildWars2.TradingPost.FindOutbidOrders
{
    public class Program
    {
        private const string m_SEARCH_STRING = "-live.ncplatform.net/authenticate?session_key=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&source=/";
        private static readonly Wildcards m_SEARCH_WILDCARDS = new Wildcards(new string[] { "X", "[A-Z0-9]" });
        private static readonly Regex m_SESSION_KEY_REGEX = new Regex("session_key=([0-9A-f]{8}-[0-9A-f]{4}-[0-9A-f]{4}-[0-9A-f]{4}-[0-9A-f]{12})", RegexOptions.Compiled);

        private const int m_NEW_LISTING_BUFFER = 100;

        private const string m_PROCESS_NAME = "Gw2";

        public static void Main(string[] args)
        {
            try
            {
                Console.Write("Checking for existing key:        ");
                string key = SessionManager.GetInstance().GameSession.Key;
                ok();
            }
            catch (GameSessionRequiredException)
            {
                error();

                Console.Write("Locating Guild Wars 2 process:    ");
                Process gw2 = findProcess(m_PROCESS_NAME);
                if (gw2 == null) { error(); return; }
                else { ok(); }

                Console.Write("Locating game session key:        ");
                string key = findSessionKey(gw2);
                if (key == null) { error(); return; }
                else { ok(); }
                SessionManager.GetInstance().GameSession = new Session(key, true);
            }
            
            Console.Write("Retreiving filtered buy listings: ");
            IList<Listing> buyOrders = new MeRequest(MeRequest.TimeType.NOW, MeRequest.ListingType.BUY)
                    .ExecuteAll()
                    .SelectMany(r => r.Listings)
                    .ToList();
            ok();

            Console.Write("Filter outbid listings:           ");
            IDictionary<int, ItemData> items = new Dictionary<int, ItemData>();
            IList<Listing> filteredListings = buyOrders
                    .Where((listing) =>
                        {
                            if (!items.ContainsKey(listing.DataId))
                            {
                                ItemDataResponse response = new ItemDataRequest(listing.DataId).Execute();
                                if (response == null)
                                    return false;

                                items[listing.DataId] = response.Result;
                            }

                            return items[listing.DataId].MaxOfferUnitPrice <= listing.UnitPrice;
                        })
                    .ToList();
            ok();

            if (filteredListings.Count > 0)
            {
                Console.WriteLine("{0} of your buy orders have not been outbid.", filteredListings.Count);
                Console.WriteLine("Orders are displayed in descending order from the date the order was placed.");
                Console.WriteLine("Press any key to view the next order in the list, or Esc to exit.");
                Console.WriteLine("{0,-10} {1}", "Price", "Name");

                foreach (Listing listing in filteredListings)
                {
                    Console.WriteLine(string.Format("{0,10} {1}",
                            listing.BuyPrice.ToString("#### ## #0").Trim(),
                            listing.Name));
                    
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                        break;
                }
            }
            else
            {
                Console.WriteLine("All of your buy orders have been outbid. Press any key to exit.");
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

        private static Process findProcess(string name)
        {
            Process p = Process.GetProcessesByName(name).FirstOrDefault();
            while (p == null)
            {
                Thread.Sleep(1000);
                p = Process.GetProcessesByName(name).FirstOrDefault();
            }
            return p;
        }

        private static string findSessionKey(Process p)
        {
            string key = null;

            MemorySearcher ms = new MemorySearcher(p);

            try
            {
                ms.BuildCache();
                string url = ms.FindString(m_SEARCH_STRING, m_SEARCH_WILDCARDS);
                while (url == null)
                {
                    Thread.Sleep(1000);
                    ms.BuildCache();
                    url = ms.FindString(m_SEARCH_STRING, m_SEARCH_WILDCARDS);
                }

                Match m = m_SESSION_KEY_REGEX.Match(url);
                if (m.Success)
                {
                    key = m.Groups[1].Value;
                }
            }
            finally
            {
                ms.ClearCache();
            }

            return key;
        }
    }
}
