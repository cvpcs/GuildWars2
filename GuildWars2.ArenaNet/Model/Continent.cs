using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class Continent
    {
        public string Name { get; set; }

        public List<int> ContinentDims { get; set; }

        public int MinZoom { get; set; }

        public int MaxZoom { get; set; }

        public List<int> Floors { get; set; }
    }
}
