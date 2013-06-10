using System;

namespace GuildWars2.ArenaNet.API
{
    public class RecipesRequest : Request<RecipesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + VERSION + "/recipes.json"; }
        }
    }
}
