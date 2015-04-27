using System;

namespace GuildWars2.ArenaNet.API.V1
{
    public class RecipesRequest : Request<RecipesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/recipes.json"; }
        }
    }
}
