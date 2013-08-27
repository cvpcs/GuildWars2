using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class WorldNamesRequest : TranslatableRequest<WorldNamesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/world_names.json"; }
        }

        public WorldNamesRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
