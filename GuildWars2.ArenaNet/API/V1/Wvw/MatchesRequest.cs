using System;

namespace GuildWars2.ArenaNet.API.V1.Wvw
{
    public class MatchesRequest : Request<MatchesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/wvw/matches.json"; }
        }
    }
}
