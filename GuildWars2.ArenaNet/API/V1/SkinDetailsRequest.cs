using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class SkinDetailsRequest : TranslatableRequest<SkinDetailsResponse>
    {
        public int SkinId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["skin_id"] = SkinId.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/skin_details.json"; }
        }

        public SkinDetailsRequest(int skin_id, LanguageCode lang = LanguageCode.EN)
            : base(lang)
        {
            SkinId = skin_id;
        }
    }
}
