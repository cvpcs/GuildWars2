using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model.Wvw
{
    public class MapState
    {
        public string Type { get; set; }
        public MapType TypeEnum
        {
            get
            {
                MapType type;
                if (Enum.TryParse<MapType>(Type, true, out type))
                    return type;

                return MapType.Invalid;
            }
        }

        public List<int> Scores { get; set; }

        public List<ObjectiveState> Objectives { get; set; }
    }
}
