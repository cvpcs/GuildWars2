using System;
using System.Collections.Generic;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    public class EventScheduleRequest : Request<EventScheduleResponse>
    {
        protected override string APIPath
        {
            get { return "/gw2/api/eventschedule"; }
        }
    }
}
