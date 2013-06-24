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
        public GameType GameTypesEnum
        {
            get
            {
                GameType typeAll = GameType.None;

                foreach(string item in GameTypes)
                {
                    GameType type;
                    if (Enum.TryParse<GameType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= GameType.Invalid;
                }

                return typeAll;
            }
        }
 
        public List<string> Flags { get; set; }
        public FlagType FlagsEnum
        {
            get
            {
                FlagType typeAll = FlagType.None;

                foreach(string item in Flags)
                {
                    FlagType type;
                    if (Enum.TryParse<FlagType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= FlagType.Invalid;
                }

                return typeAll;
            }
        }

        public List<string> Restrictions { get; set; }
        public RestrictionType RestrictionsEnum
        {
            get
            {
                RestrictionType typeAll = RestrictionType.None;

                foreach (string item in Flags)
                {
                    RestrictionType type;
                    if (Enum.TryParse<RestrictionType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= RestrictionType.Invalid;
                }

                return typeAll;
            }
        }

        // armor?
        // weapon?
        // other stats?
    }
}
