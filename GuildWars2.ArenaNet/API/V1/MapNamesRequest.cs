﻿using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class MapNamesRequest : TranslatableRequest<MapNamesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/map_names.json"; }
        }

        public MapNamesRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
