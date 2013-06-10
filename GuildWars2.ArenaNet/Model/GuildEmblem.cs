using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class GuildEmblem
    {
        public int BackgroundId { get; set; }

        public int ForegroundId { get; set; }

        // dafuq?
        public List<int> Flags { get; set; }

        public int BackgroundColorId { get; set; }

        public int ForegroundPrimaryColorId { get; set; }

        public int ForegroundSecondaryColorId { get; set; }
    }
}
