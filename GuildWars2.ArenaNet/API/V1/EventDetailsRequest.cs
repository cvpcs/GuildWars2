using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class EventDetailsRequest : TranslatableRequest<EventDetailsResponse>
    {
        public Guid? EventId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                if (EventId.HasValue)
                    parameters["event_id"] = EventId.Value.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/event_details.json"; }
        }

        public EventDetailsRequest(Guid? event_id = null, LanguageCode lang = LanguageCode.EN)
            : base(lang)
        {
            EventId = event_id;
        }
    }
}
