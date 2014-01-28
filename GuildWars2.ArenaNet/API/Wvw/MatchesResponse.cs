using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model.Wvw;

namespace GuildWars2.ArenaNet.API.Wvw
{
    public class MatchesResponse
    {
        [JsonProperty("wvw_matches")]
        public List<Match> WvwMatches { get; set; }
    }
}
