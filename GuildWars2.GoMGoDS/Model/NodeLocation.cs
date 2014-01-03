using System;
using System.Runtime.Serialization;

namespace GuildWars2.GoMGoDS.Model
{
    [DataContract]
    public class NodeLocation
    {
        [DataMember(Name = "x")]
        public int X;

        [DataMember(Name = "y")]
        public int Y;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "type")]
        public string Type;
    }
}
