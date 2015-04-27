using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum DisciplineType
    {
        None          = 0x000,
        Invalid       = 0x001,
        Artificer     = 0x002,
        Armorsmith    = 0x004,
        Chef          = 0x008,
        Huntsman      = 0x010,
        Jeweler       = 0x020,
        Leatherworker = 0x040,
        Tailor        = 0x080,
        Weaponsmith   = 0x100
    }
}
