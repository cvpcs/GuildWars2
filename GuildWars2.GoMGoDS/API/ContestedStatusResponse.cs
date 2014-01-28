using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.GoMGoDS.API
{
    public class ContestedStatusResponse
    {
        [JsonProperty("build")]
        public int Build;

        [JsonProperty("timestamp")]
        public long Timestamp;

        [JsonProperty("locations")]
        public List<ContestedLocationStatus> Locations;
    }
}
