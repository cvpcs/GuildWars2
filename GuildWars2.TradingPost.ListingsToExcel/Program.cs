using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using LibMemorySearch;

using GuildWars2.TradingPost.API;
using GuildWars2.TradingPost.Exceptions;
using GuildWars2.TradingPost.Model;

namespace GuildWars2.TradingPost.ListingsToExcel
{
    public class Program
    {
        private const string m_SEARCH_STRING = "-live.ncplatform.net/authenticate?session_key=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&source=/";
        private static readonly Wildcards m_SEARCH_WILDCARDS = new Wildcards(new string[] { "X", "[A-Z0-9]" });
        private static readonly Regex m_SESSION_KEY_REGEX = new Regex("session_key=([0-9A-f]{8}-[0-9A-f]{4}-[0-9A-f]{4}-[0-9A-f]{4}-[0-9A-f]{12})", RegexOptions.Compiled);

        private static readonly string m_EXCEL_DB = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Guild Wars Listings.xlsx"
            );

        private const int m_NEW_LISTING_BUFFER = 200;

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
            
            Console.Write("Loading excel database:           ");
            ExcelDatabase db = ExcelDatabase.LoadFromFile(m_EXCEL_DB);
            ok();
            
            Console.Write("Updating listings:                ");
            updateListings(db);
            ok();
            
            Console.Write("Saving database to file:          ");
            db.SaveToFile(m_EXCEL_DB);
            ok();
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

        private static void updateListings(ExcelDatabase db)
        {
            // buckets to put orders into
            IDictionary<long, ListingInfo> buy_past_fulfilled = new Dictionary<long, ListingInfo>();
            IDictionary<long, ListingInfo> buy_past_cancelled = new Dictionary<long, ListingInfo>();
            IDictionary<long, ListingInfo> buy_now = new Dictionary<long, ListingInfo>();
            IDictionary<long, ListingInfo> sell_past_fulfilled = new Dictionary<long, ListingInfo>();
            IDictionary<long, ListingInfo> sell_past_cancelled = new Dictionary<long, ListingInfo>();
            IDictionary<long, ListingInfo> sell_now = new Dictionary<long, ListingInfo>();

            // siphon through the current database, throwing stuff into the proper buckets
            foreach (ListingInfo listing in db.Listings.Values)
            {
                switch (listing.Type)
                {
                    case ListingInfo.ListingType.BUY:
                        if (listing.Fulfilled)
                            buy_past_fulfilled[listing.ListingId] = listing;
                        else if (listing.Cancelled)
                            buy_past_cancelled[listing.ListingId] = listing;
                        else
                            buy_now[listing.ListingId] = listing;
                        break;
                    case ListingInfo.ListingType.SELL:
                        if (listing.Fulfilled)
                            sell_past_fulfilled[listing.ListingId] = listing;
                        else if (listing.Cancelled)
                            sell_past_cancelled[listing.ListingId] = listing;
                        else
                            sell_now[listing.ListingId] = listing;
                        break;
                    default:
                        break;
                }
            }

            // clear the excel database (it will be refreshed)
            db.Listings.Clear();

            /***************************** BUY ORDERS *******************************************/
            // get all of our now listings
            IDictionary<long, ListingInfo> buy_now_result = resultsToListingDictionary(
                    new MeRequest(MeRequest.TimeType.NOW, MeRequest.ListingType.BUY).ExecuteAll(),
                    MeRequest.ListingType.BUY);

            // get our new past listings
            IDictionary<long, ListingInfo> buy_past_result = resultsToListingDictionary(
                    new MeRequest(MeRequest.TimeType.PAST, MeRequest.ListingType.BUY).GetNew(buy_past_fulfilled.Count - m_NEW_LISTING_BUFFER, 100),
                    MeRequest.ListingType.BUY);

            // clear buy now and cancelled listings. we simply don't care with buy orders
            buy_now.Clear();
            buy_past_cancelled.Clear();

            // add our now buy orders
            foreach (ListingInfo info in buy_now_result.Values)
                buy_now[info.ListingId] = info;

            // add our new past buy orders
            foreach (ListingInfo info in buy_past_result.Values)
                buy_past_fulfilled[info.ListingId] = info;

            /***************************** SELL ORDERS ******************************************/
            // get all of our now listings
            IDictionary<long, ListingInfo> sell_now_result = resultsToListingDictionary(
                    new MeRequest(MeRequest.TimeType.NOW, MeRequest.ListingType.SELL).ExecuteAll(),
                    MeRequest.ListingType.SELL);

            // get our new past listings
            IDictionary<long, ListingInfo> sell_past_result = resultsToListingDictionary(
                    new MeRequest(MeRequest.TimeType.PAST, MeRequest.ListingType.SELL).GetNew(sell_past_fulfilled.Count - m_NEW_LISTING_BUFFER, 100),
                    MeRequest.ListingType.SELL);

            // find our cancelled orders
            foreach (ListingInfo listing in sell_now.Values)
            {
                if (!sell_now_result.ContainsKey(listing.ListingId) &&
                    !sell_past_fulfilled.ContainsKey(listing.ListingId))
                {
                    listing.Cancelled = true;
                    sell_past_cancelled[listing.ListingId] = listing;
                }
            }

            // clear now, any current "now" orders will be refilled
            sell_now.Clear();

            // add our now sell orders
            foreach (ListingInfo info in sell_now_result.Values)
                sell_now[info.ListingId] = info;

            // add our new past sell orders
            foreach (ListingInfo info in sell_past_result.Values)
                sell_past_fulfilled[info.ListingId] = info;

            /***************************** COMBINE **********************************************/
            foreach (ListingInfo info in buy_past_fulfilled.Values)
                db.Listings[info.ListingId] = info;

            foreach (ListingInfo info in buy_now.Values)
                db.Listings[info.ListingId] = info;

            foreach (ListingInfo info in sell_past_fulfilled.Values)
                db.Listings[info.ListingId] = info;

            foreach (ListingInfo info in sell_past_cancelled.Values)
                db.Listings[info.ListingId] = info;

            foreach (ListingInfo info in sell_now.Values)
                db.Listings[info.ListingId] = info;
        }

        private static IDictionary<long, ListingInfo> resultsToListingDictionary(IList<MeResponse> results, MeRequest.ListingType type)
        {
            IDictionary<long, ListingInfo> listings = new Dictionary<long, ListingInfo>();

            foreach (MeResponse response in results)
            {
                foreach (Listing listing in response.Listings)
                {
                    ListingInfo info = ListingInfo.FromListing(listing, type);
                    listings[info.ListingId] = info;
                }
            }

            return listings;
        }
    }
}
