using System;
using System.Collections.Generic;

namespace GuildWars2.SyntaxError.Model
{
    public class GuildBountyPath
    {
        public PathDirectionType Direction { get; set; }

        public List<List<double>> Points { get; set; }
    }
}
