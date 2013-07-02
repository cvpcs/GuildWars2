using System;
using System.Collections.Generic;

using GuildWars2.SyntaxError.Model;

namespace GuildWars2.SyntaxError.API
{
    public class EventTimerDataRequest : Request<EventTimerDataResponse>
    {
        protected override string APIPath
        {
            get { return "/gw2/eventtimer/data"; }
        }
    }
}
