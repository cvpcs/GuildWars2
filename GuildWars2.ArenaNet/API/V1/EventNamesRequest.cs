using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class EventNamesRequest : TranslatableRequest<EventNamesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/event_names.json"; }
        }

        public EventNamesRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
