using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1.Wvw
{
    public class MatchDetails
    {
        [JsonProperty("match_id")]
        public string MatchId { get; set; }

        [JsonProperty("scores")]
        public List<int> Scores { get; set; }

        [JsonProperty("maps")]
        public List<MapState> Maps { get; set; }
    }
}
