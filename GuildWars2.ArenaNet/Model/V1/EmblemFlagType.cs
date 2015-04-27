using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum EmblemFlagType
    {
        None                     = 0x00,
        Invalid                  = 0x01,
        FlipBackgroundHorizontal = 0x02,
        FlipBackgroundVertical   = 0x04,
        FlipForegroundHorizontal = 0x08,
        FlipForegroundVertical   = 0x10
    }
}
