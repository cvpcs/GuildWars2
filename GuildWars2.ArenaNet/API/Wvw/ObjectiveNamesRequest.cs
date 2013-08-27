using System;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API.Wvw
{
    public class ObjectiveNamesRequest : TranslatableRequest<ObjectiveNamesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/wvw/objective_names.json"; }
        }

        public ObjectiveNamesRequest(LanguageCode lang = LanguageCode.EN)
            : base(lang)
        { }
    }
}
