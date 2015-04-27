using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class MapsRequest : TranslatableRequest<MapsResponse>
    {
        public int? MapId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                if (MapId.HasValue)
                    parameters["map_id"] = MapId.Value.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/maps.json"; }
        }

        public MapsRequest(int? map_id = null, LanguageCode lang = LanguageCode.EN)
            : base(lang)
        {
            MapId = map_id;
        }
    }
}
