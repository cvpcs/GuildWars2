using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class MapNamesRequest : TranslatableRequest<MapNamesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + VERSION + "/map_names.json"; }
        }

        public MapNamesRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
