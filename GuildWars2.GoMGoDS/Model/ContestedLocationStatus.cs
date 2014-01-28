using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.GoMGoDS.Model
{
    public class ContestedLocationStatus
    {
        [JsonIgnore]
        public string Id { get { return Abbreviation.ToLower(); } }

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("abbreviation")]
        public string Abbreviation;

        [JsonProperty("open_on")]
        public List<int> OpenOn;

        [JsonProperty("defend_on")]
        public List<int> DefendOn;

        [JsonProperty("capture_on")]
        public List<int> CaptureOn;
    }
}
