using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum EventFlagType
    {
        None       = 0x0,
        Invalid    = 0x1,
        GroupEvent = 0x2,
        MapWide    = 0x4
    }
}
