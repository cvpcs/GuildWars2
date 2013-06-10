using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class Color
    {
        public string Name { get; set; }

        public List<int> BaseRgb { get; set; }

        public MaterialColor Cloth { get; set; }

        public MaterialColor Leather { get; set; }

        public MaterialColor Metal { get; set; }
    }
}
