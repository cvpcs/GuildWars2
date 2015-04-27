using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public abstract class TranslatableRequest<T> : Request<T>
        where T : class, new()
    {
        public LanguageCode Language { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["lang"] = Enum.GetName(typeof(LanguageCode), Language).ToLower();

                return parameters;
            }
        }

        public TranslatableRequest(LanguageCode lang = LanguageCode.EN)
        {
            Language = lang;
        }

    }
}
