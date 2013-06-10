using System;

namespace GuildWars2.ArenaNet.Model
{
    [Flags]
    public enum FlagType
    {
        Invalid,
        AccountBound,
        HideSuffix,
        NoMysticForge,
        NoSalvage,
        NoSell,
        NoUnderwater,
        NotUpgradeable,
        SoulBindOnUse,
        SoulbindOnAcquire,
        Unique
    }
}
