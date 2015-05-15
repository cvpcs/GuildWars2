using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class SkinDetailsWeapon
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
        public WeaponType TypeEnum
        {
            get
            {
                WeaponType type;
                if (Enum.TryParse<WeaponType>(Type, true, out type))
                    return type;

                return WeaponType.Invalid;
            }
        }

        [JsonProperty("damage_type")]
        public string DamageType { get; set; }
        [JsonIgnore]
        public WeaponDamageType DamageTypeEnum
        {
            get
            {
                WeaponDamageType type;
                if (Enum.TryParse<WeaponDamageType>(DamageType, true, out type))
                    return type;

                return WeaponDamageType.Invalid;
            }
        }
    }
}
