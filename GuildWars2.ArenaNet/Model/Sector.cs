using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class Sector : MappedModel
    {
        public int SectorId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }
    }
}
