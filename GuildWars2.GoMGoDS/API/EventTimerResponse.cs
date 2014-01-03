using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    [DataContract]
    public class EventTimerResponse
    {
        [DataMember(Name = "build")]
        public int Build;

        [DataMember(Name = "timestamp")]
        public long Timestamp;

        [DataMember(Name = "events")]
        public List<MetaEventStatus> Events;
    }
}
