using System;
using System.Collections.Generic;

namespace GuildWars2.SyntaxError.Model
{
    public class GuildBounty
    {
        public string Name { get; set; }

        public int MapId { get; set; }

        public List<List<double>> Spawns { get; set; }

        public List<GuildBountyPath> Paths { get; set; }
    }
}
