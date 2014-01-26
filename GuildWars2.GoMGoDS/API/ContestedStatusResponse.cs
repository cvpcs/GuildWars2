using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    [DataContract]
    public class ContestedStatusResponse
    {
        [DataMember(Name = "build")]
        public int Build;

        [DataMember(Name = "timestamp")]
        public long Timestamp;

        [DataMember(Name = "locations")]
        public List<ContestedLocationStatus> Locations;
    }
}
