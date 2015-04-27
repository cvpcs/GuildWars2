using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class Task : MappedModel
    {
        [JsonProperty("task_id")]
        public int TaskId { get; set; }

        [JsonProperty("objective")]
        public string Objective { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
