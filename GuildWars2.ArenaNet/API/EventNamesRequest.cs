using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class EventNamesRequest : TranslatableRequest<EventNamesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + VERSION + "/event_names.json"; }
        }

        public EventNamesRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
