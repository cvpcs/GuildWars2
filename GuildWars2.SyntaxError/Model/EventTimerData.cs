using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuildWars2.SyntaxError.Model
{
    [DataContract]
    public class EventTimerData
    {
        [DataMember(Name = "build")]
        public int Build;

        [DataMember(Name = "timestamp")]
        public long Timestamp;

        [DataMember(Name = "events")]
        public List<MetaEventStatus> Events;
    }
}
