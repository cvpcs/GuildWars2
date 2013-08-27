using System;

using GuildWars2.ArenaNet.API;

namespace GuildWars2.ArenaNet.API.Wvw
{
    public class MatchesRequest : Request<MatchesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/wvw/matches.json"; }
        }
    }
}
