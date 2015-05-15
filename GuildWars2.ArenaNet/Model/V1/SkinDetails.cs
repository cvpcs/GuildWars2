using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class SkinDetails
    {
        [JsonProperty("skin_id")]
        public int SkinId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
        public SkinType TypeEnum
        {
            get
            {
                SkinType type;
                if (Enum.TryParse<SkinType>(Type, true, out type))
                    return type;

                return SkinType.Invalid;
            }
        }

        [JsonProperty("flags")]
        public List<string> Flags { get; set; }
        [JsonIgnore]
        public SkinFlagType FlagsEnum
        {
            get
            {
                SkinFlagType typeAll = SkinFlagType.None;

                foreach(string item in Flags)
                {
                    SkinFlagType type;
                    if (Enum.TryParse<SkinFlagType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= SkinFlagType.Invalid;
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

        [JsonProperty("armor")]
        public SkinDetailsArmor Armor { get; set; }

        [JsonProperty("weapon")]
        public SkinDetailsWeapon Weapon { get; set; }
    }
}
