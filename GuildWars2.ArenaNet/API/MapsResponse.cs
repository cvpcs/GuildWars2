using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class MapsResponse
    {
        public Dictionary<string, MapDetails> Maps { get; set; }
    }
}
