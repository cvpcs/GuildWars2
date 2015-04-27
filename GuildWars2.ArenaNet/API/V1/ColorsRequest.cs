using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class ColorsRequest : TranslatableRequest<ColorsResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/colors.json"; }
        }

        public ColorsRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
