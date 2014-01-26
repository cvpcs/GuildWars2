using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuildWars2.GoMGoDS.Model
{
    [DataContract]
    public class ContestedLocationStatus
    {
        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "abbreviation")]
        public string Abbreviation;

        [DataMember(Name = "open_on")]
        public Dictionary<string, string> OpenOn;

        [DataMember(Name = "defend_on")]
        public Dictionary<string, int> DefendOn;

        [DataMember(Name = "capture_on")]
        public Dictionary<string, double> CaptureOn;
    }
}
