using System;

namespace GuildWars2.TradingPost.ListingsToExcel
{
    public static class BugFixes
    {
        public static void Fix_001(ref string rarity)
        {
            long test = 0;
            if (long.TryParse(rarity, out test) && test.ToString() == rarity)
                rarity = "Unknown";
        }
    }
}
