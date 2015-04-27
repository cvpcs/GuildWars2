using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.API.V1
{
    [Obsolete("This API has been depricated and the endpoint has been disabled.")]
    public class EventsRequest : Request<EventsResponse>
    {
        public int? WorldId { get; set; }
        public int? MapId { get; set; }
        public Guid? EventId { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                if (WorldId.HasValue)
                    parameters["world_id"] = WorldId.Value.ToString();
                if (MapId.HasValue)
                    parameters["map_id"] = MapId.Value.ToString();
                if (EventId.HasValue)
                    parameters["event_id"] = EventId.Value.ToString();

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/events.json"; }
        }

        public EventsRequest(int? world_id = null, int? map_id = null, Guid? event_id = null)
        {
            WorldId = world_id;
            MapId = map_id;
            EventId = event_id;
        }
    }
}
