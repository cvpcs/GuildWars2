using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuildWars2.GoMGoDS.Model
{
    [DataContract]
    public class ContestedLocationStatus
    {
        public string Id { get { return Abbreviation.ToLower(); } }

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "abbreviation")]
        public string Abbreviation;

        [DataMember(Name = "open_on")]
        public List<int> OpenOn;

        [DataMember(Name = "defend_on")]
        public List<int> DefendOn;

        [DataMember(Name = "capture_on")]
        public List<int> CaptureOn;
    }
}
