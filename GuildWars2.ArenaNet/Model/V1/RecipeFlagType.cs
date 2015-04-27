using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum RecipeFlagType
    {
        None            = 0x0,
        Invalid         = 0x1,
        AutoLearned     = 0x2,
        LearnedFromItem = 0x4
    }
}
