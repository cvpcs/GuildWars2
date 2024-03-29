﻿using System;

using GuildWars2.ArenaNet.API.V1;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class EventStatePublisher : PublisherBase<EventsResponse>
    {
        protected static TimeSpan p_PollRate = new TimeSpan(0, 0, 30);

        public EventStatePublisher()
            : base(p_PollRate)
        { }

        protected override bool UpdateData()
        {
            EventsResponse events = new EventsRequest().Execute();

            if (events != null)
            {
                m_Data = events;
                return true;
            }

            return false;
        }
    }
}
