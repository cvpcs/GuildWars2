using System;

namespace GuildWars2.ArenaNet.Model.V1
{
    [Flags]
    public enum GameType
    {
        None     = 0x00,
        Invalid  = 0x01,
        Activity = 0x02,
        Dungeon  = 0x04,
        Pve      = 0x08,
        Pvp      = 0x10,
        PvpLobby = 0x20,
        Wvw      = 0x40
    }
}
