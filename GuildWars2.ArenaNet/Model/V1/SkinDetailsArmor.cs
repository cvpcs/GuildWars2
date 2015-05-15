using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class SkinDetailsArmor
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
        public ArmorType TypeEnum
        {
            get
            {
                ArmorType type;
                if (Enum.TryParse<ArmorType>(Type, true, out type))
                    return type;

                return ArmorType.Invalid;
            }
        }

        [JsonProperty("weight_class")]
        public string WeightClass { get; set; }
        [JsonIgnore]
        public ArmorWeightClassType WeightClassEnum
        {
            get
            {
                ArmorWeightClassType type;
                if (Enum.TryParse<ArmorWeightClassType>(WeightClass, true, out type))
                    return type;

                return ArmorWeightClassType.Invalid;
            }
        }
    }
}
