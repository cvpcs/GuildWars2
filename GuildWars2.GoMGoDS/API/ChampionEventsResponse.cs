using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuildWars2.GoMGoDS.API
{
    [DataContract]
    public class ChampionEventsResponse
    {
        [DataMember(Name = "champion_events")]
        public List<Guid> ChampionEvents;
    }
}
