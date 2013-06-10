using System;
using System.Collections.Generic;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.TradingPost.ListingsToExcel
{
    public static class SpidyData
    {
        private static string m_RARITY_UNKNOWN = "Unknown";
        private static IDictionary<int, string> m_RARITIES = null;
        public static IDictionary<int, string> RARITIES
        {
            get
            {
                if (m_RARITIES == null)
                {
                    m_RARITIES = new Dictionary<int, string>();

                    RarityListResponse response = new RarityListRequest().Execute();
                    if (response != null)
                    {
                        foreach (Rarity rarity in response.Results)
                        {
                            m_RARITIES.Add(rarity.Id, rarity.Name);
                        }
                    }
                }

                return m_RARITIES;
            }
        }

        public static int GetRarityIdFromName(string rarity)
        {
            foreach (KeyValuePair<int, string> element in RARITIES)
            {
                if (element.Value == rarity)
                    return element.Key;
            }

            return -1;
        }
        public static string GetRarityNameFromId(int rarity)
        {
            if (RARITIES.ContainsKey(rarity))
                return RARITIES[rarity];

            return m_RARITY_UNKNOWN;
        }
    }
}
