using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model.Wvw
{
    public class MatchDetails
    {
        public string MatchId { get; set; }

        public List<int> Scores { get; set; }

        public List<MapState> Maps { get; set; }
    }
}
