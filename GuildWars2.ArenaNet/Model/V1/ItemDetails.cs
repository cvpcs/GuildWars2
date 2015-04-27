using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class ItemDetails
    {
        [JsonProperty("item_id")]
        public int ItemId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("vendor_value")]
        public int VendorValue { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
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

        [JsonProperty("rarity")]
        public string Rarity { get; set; }
        [JsonIgnore]
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

        [JsonProperty("game_types")]
        public List<string> GameTypes { get; set; }
        [JsonIgnore]
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

        [JsonProperty("flags")]
        public List<string> Flags { get; set; }
        [JsonIgnore]
        public ItemFlagType FlagsEnum
        {
            get
            {
                ItemFlagType typeAll = ItemFlagType.None;

                foreach(string item in Flags)
                {
                    ItemFlagType type;
                    if (Enum.TryParse<ItemFlagType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= ItemFlagType.Invalid;
                }

                return typeAll;
            }
        }

        [JsonProperty("restrictions")]
        public List<string> Restrictions { get; set; }
        [JsonIgnore]
        public RestrictionType RestrictionsEnum
        {
            get
            {
                RestrictionType typeAll = RestrictionType.None;

                foreach (string item in Restrictions)
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

        [JsonProperty("icon_file_id")]
        public int IconFileId { get; set; }

        [JsonProperty("icon_file_signature")]
        public string IconFileSignature { get; set; }

        [JsonIgnore]
        public AssetFile IconFile
        {
            get
            {
                return new AssetFile()
                    {
                        FileId = IconFileId,
                        Signature = IconFileSignature
                    };
            }
        }

        [JsonProperty("default_skin")]
        public int DefaultSkinId { get; set; }

        // armor?
        // weapon?
        // other stats?
    }
}
