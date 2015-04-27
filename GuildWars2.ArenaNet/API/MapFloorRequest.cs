using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class MapFloorRequest : TranslatableRequest<MapFloorResponse>
    {
        public int ContinentId { get; set; }
        
        public int Floor { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["continent_id"] = ContinentId.ToString();
                parameters["floor"] = Floor.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/map_floor.json"; }
        }

        public MapFloorRequest(int continent_id, int floor, LanguageCode lang = LanguageCode.EN)
            : base(lang)
        {
            ContinentId = continent_id;
            Floor = floor;
        }
    }
}
