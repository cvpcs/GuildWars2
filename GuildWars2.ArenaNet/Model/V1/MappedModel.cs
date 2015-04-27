using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class MappedModel
    {
        [JsonProperty("coord")]
        public List<double> Coord { get; set; }
    }
}
