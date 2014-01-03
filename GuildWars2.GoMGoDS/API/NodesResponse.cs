using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    [DataContract]
    public class NodesResponse
    {
        [DataMember(Name = "world_id")]
        public int WorldId;

        [DataMember(Name = "map_id")]
        public int MapId;

        [DataMember(Name = "timestamp")]
        public long Timestamp;

        [DataMember(Name = "nodes")]
        public List<NodeLocation> Nodes;
    }
}
