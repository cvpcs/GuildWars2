using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    public class NodesResponse
    {
        [JsonProperty("world_id")]
        public int WorldId;

        [JsonProperty("map_id")]
        public int MapId;

        [JsonProperty("timestamp")]
        public long Timestamp;

        [JsonProperty("nodes")]
        public List<NodeLocation> Nodes;
    }
}
