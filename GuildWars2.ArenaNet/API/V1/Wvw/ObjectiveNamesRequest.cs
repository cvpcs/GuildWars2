using System;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1.Wvw
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
