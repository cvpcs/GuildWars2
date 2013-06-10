using System;

namespace GuildWars2.ArenaNet.API
{
    public class BuildRequest : Request<BuildResponse>
    {
        protected override string APIPath
        {
            get { return "/" + VERSION + "/build.json"; }
        }
    }
}
