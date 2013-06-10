using System;

namespace GuildWars2.ArenaNet.Model
{
    public enum EventStateType
    {
        Invalid,    // unknown state
        Active,     // event is running
        Success,    // event succeeded
        Fail,       // event failed
        Warmup,     // event is inactive and will only become active once certain criteria are met
        Preparation // event warmup criteria are met, but certain auto-running activities have not been completed yet to become active
    }
}
