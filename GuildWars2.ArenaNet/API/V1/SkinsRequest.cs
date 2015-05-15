using System;

namespace GuildWars2.ArenaNet.API.V1
{
    public class SkinsRequest : Request<SkinsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/skins.json"; }
        }
    }
}
