using System;

namespace GuildWars2.ArenaNet.Model
{
    [Flags]
    public enum RestrictionType
    {
        None,
        Invalid,
        Asura,
        Charr,
        Human,
        Norn,
        Sylvari,
        Guardian,
        Warrior
    }
}
