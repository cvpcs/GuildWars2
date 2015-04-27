using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class ContinentsRequest : TranslatableRequest<ContinentsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/continents.json"; }
        }

        public ContinentsRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
