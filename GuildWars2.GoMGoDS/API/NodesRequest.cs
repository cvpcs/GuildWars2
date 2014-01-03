using System;
using System.Collections.Generic;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    public class NodesRequest : Request<NodesResponse>
    {
        public int WorldId { get; set; }
        public int MapId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["world_id"] = WorldId.ToString();
                parameters["map_id"] = MapId.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/gw2/api/nodes"; }
        }
        
        public NodesRequest(int world_id, int map_id)
            : base()
        {
            WorldId = world_id;
            MapId = map_id;
        }
    }
}
