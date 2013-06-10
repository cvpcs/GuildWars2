using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.Spidy.API;
using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.MFMarketSpeculation
{
    public abstract class BaseSpeculation
    {
        protected const double PromotionRate = 0.2;

        public abstract string Name { get; }
        public abstract void Run();

        private static IList<Rarity> m_Rarities = null;
        protected IList<Rarity> Rarities
        {
            get
            {
                if (m_Rarities == null)
                    m_Rarities = new RarityListRequest().Execute().Results;

                return m_Rarities;
            }
        }

        private static IList<ItemType> m_Types = null;
        protected IList<ItemType> Types
        {
            get
            {
                if (m_Types == null)
                    m_Types = new TypeListRequest().Execute().Results;

                return m_Types;
            }
        }

        protected Rarity GetRarity(string rarity)
        {
            IList<Rarity> rarities = Rarities.Where(r => r.Name == rarity).ToList();
            if (rarities.Count == 1)
                return rarities[0];
            else
                return new Rarity() { Id = -1, Name = rarity };
        }

        protected Rarity GetPromotedRarity(Rarity rarity)
        {
            switch (rarity.Name)
            {
                case "Common":
                    return GetRarity("Fine");
                case "Fine":
                    return GetRarity("Masterwork");
                case "Masterwork":
                    return GetRarity("Rare");
                case "Rare":
                    return GetRarity("Exotic");
                default:
                    return rarity;
            }
        }

        protected ItemType GetItemType(string type)
        {
            IList<ItemType> types = Types.Where(t => t.Name == type).ToList();
            if (types.Count == 1)
                return types[0];
            else
                return new ItemType() { Id = -1, Name = type };
        }
    }
}
