using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.API
{
    public class RecipeDetailsRequest : Request<RecipeDetailsResponse>
    {
        public int RecipeId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["recipe_id"] = RecipeId.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/recipe_details.json"; }
        }

        public RecipeDetailsRequest(int recipe_id)
        {
            RecipeId = recipe_id;
        }
    }
}
