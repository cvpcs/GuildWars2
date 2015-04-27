using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum RestrictionType
    {
        None         = 0x0000,
        Invalid      = 0x0001,
        Asura        = 0x0002,
        Charr        = 0x0004,
        Human        = 0x0008,
        Norn         = 0x0010,
        Sylvari      = 0x0020,
        Guardian     = 0x0040,
        Warrior      = 0x0080,
        Mesmer       = 0x0100,
        Elementalist = 0x0200,
        Thief        = 0x0400,
        Engineer     = 0x0800,
        Necromancer  = 0x1000,
        Ranger       = 0x2000,
        Revenant     = 0x4000
    }
}
