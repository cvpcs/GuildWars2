using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class ContinentsResponse
    {
        public Dictionary<string, Continent> Continents { get; set; }
    }
}
