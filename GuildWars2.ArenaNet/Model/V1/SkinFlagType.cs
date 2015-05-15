using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum SkinFlagType
    {
        None           = 0x0,
        Invalid        = 0x1,
        ShowInWardrobe = 0x2,
        NoCost         = 0x4,
        HideIfUnlocked = 0x8
    }
}
