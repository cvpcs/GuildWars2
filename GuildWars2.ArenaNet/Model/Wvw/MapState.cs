using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.Wvw
{
    public class MapState
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
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

        [JsonProperty("scores")]
        public List<int> Scores { get; set; }

        [JsonProperty("objectives")]
        public List<ObjectiveState> Objectives { get; set; }
    }
}
