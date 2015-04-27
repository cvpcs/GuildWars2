using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum ItemFlagType
    {
        None              = 0x0000,
        Invalid           = 0x0001,
        AccountBindOnUse  = 0x0002,
        AccountBound      = 0x0004,
        HideSuffix        = 0x0008,
        MonsterOnly       = 0x0010,
        NoMysticForge     = 0x0020,
        NoSalvage         = 0x0040,
        NoSell            = 0x0080,
        NoUnderwater      = 0x0100,
        NotUpgradeable    = 0x0200,
        SoulbindOnAcquire = 0x0400,
        SoulBindOnUse     = 0x0800,
        Unique            = 0x1000
    }
}
