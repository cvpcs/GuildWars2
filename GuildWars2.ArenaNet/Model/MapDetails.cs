using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class MapDetails
    {
        public string MapName { get; set; }

        public int MinLevel { get; set; }

        public int MaxLevel { get; set; }

        public int DefaultFloor { get; set; }

        public List<int> Floors { get; set; }

        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public int ContinentId { get; set; }

        public string ContinentName { get; set; }

        public List<List<int>> MapRect { get; set; }

        public List<List<int>> ContinentRect { get; set; }
    }
}
