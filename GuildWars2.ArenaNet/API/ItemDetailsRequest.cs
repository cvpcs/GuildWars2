using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class ItemDetailsRequest : TranslatableRequest<ItemDetailsResponse>
    {
        public int ItemId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["item_id"] = ItemId.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/item_details.json"; }
        }

        public ItemDetailsRequest(int item_id, LanguageCode lang = LanguageCode.EN)
            : base(lang)
        {
            ItemId = item_id;
        }
    }
}
