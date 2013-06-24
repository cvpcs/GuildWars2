using System;

namespace GuildWars2.ArenaNet.Model
{
    [Flags]
    public enum GameType
    {
        None,
        Invalid,
        Activity,
        Dungeon,
        Pve,
        Pvp,
        PvpLobby,
        Wvw
    }
}
