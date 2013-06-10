using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class ItemDetails
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int VendorValue { get; set; }

        public string Type { get; set; }
        public ItemType TypeEnum
        {
            get
            {
                ItemType type;
                if (Enum.TryParse<ItemType>(Type, true, out type))
                    return type;

                return ItemType.Invalid;
            }
        }

        public string Rarity { get; set; }
        public RarityType RarityEnum
        {
            get
            {
                RarityType type;
                if (Enum.TryParse<RarityType>(Rarity, true, out type))
                    return type;

                return RarityType.Invalid;
            }
        }

        public List<string> GameTypes { get; set; }
        public List<GameType> GameTypesEnum
        {
            get
            {
                List<GameType> types = new List<GameType>();

                foreach(string item in GameTypes)
                {
                    GameType type;
                    if (Enum.TryParse<GameType>(item, true, out type))
                        types.Add(type);
                    else
                        types.Add(GameType.Invalid);
                }

                return types;
            }
        }
 
        public List<string> Flags { get; set; }
        public List<FlagType> FlagsEnum
        {
            get
            {
                List<FlagType> types = new List<FlagType>();

                foreach(string item in Flags)
                {
                    FlagType type;
                    if (Enum.TryParse<FlagType>(item, true, out type))
                        types.Add(type);
                    else
                        types.Add(FlagType.Invalid);
                }

                return types;
            }
        }

        public List<string> Restrictions { get; set; }
        public List<RestrictionType> RestrictionsEnum
        {
            get
            {
                List<RestrictionType> types = new List<RestrictionType>();

                foreach(string item in Restrictions)
                {
                    RestrictionType type;
                    if (Enum.TryParse<RestrictionType>(item, true, out type))
                        types.Add(type);
                    else
                        types.Add(RestrictionType.Invalid);
                }

                return types;
            }
        }

        // armor?
        // weapon?
        // other stats?
    }
}
