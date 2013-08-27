using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
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
