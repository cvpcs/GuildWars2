using System;

using RestSharp.Deserializers;

namespace GuildWars2.ArenaNet.Model.Wvw
{
    public class Match
    {
        public string WvwMatchId { get; set; }

        public int RedWorldId { get; set; }

        public int BlueWorldId { get; set; }

        public int GreenWorldId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
