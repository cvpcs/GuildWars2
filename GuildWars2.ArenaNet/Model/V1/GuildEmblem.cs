using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class GuildEmblem
    {
        [JsonProperty("background_id")]
        public int BackgroundId { get; set; }

        [JsonProperty("foreground_id")]
        public int ForegroundId { get; set; }

        [JsonProperty("flags")]
        public List<string> Flags { get; set; }
        [JsonIgnore]
        public EmblemFlagType FlagsEnum
        {
            get
            {
                EmblemFlagType typeAll = EmblemFlagType.None;

                foreach (string item in Flags)
                {
                    EmblemFlagType type;
                    if (Enum.TryParse<EmblemFlagType>(item.Replace("_", string.Empty), true, out type))
                        typeAll |= type;
                    else
                        typeAll |= EmblemFlagType.Invalid;
                }

                return typeAll;
            }
        }

        [JsonProperty("background_color_id")]
        public int BackgroundColorId { get; set; }

        [JsonProperty("foreground_primary_color_id")]
        public int ForegroundPrimaryColorId { get; set; }

        [JsonProperty("foreground_secondary_color_id")]
        public int ForegroundSecondaryColorId { get; set; }
    }
}
