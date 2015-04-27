using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.API.V1.Wvw
{
    public class MatchDetailsRequest : Request<MatchDetailsResponse>
    {
        public string MatchId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["match_id"] = MatchId;

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/wvw/match_details.json"; }
        }

        public MatchDetailsRequest(string match_id)
        {
            MatchId = match_id;
        }
    }
}
