using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class FloorMapDetails : MapDetailsBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("points_of_interest")]
        public List<PointOfInterest> PointsOfInterest { get; set; }

        [JsonProperty("tasks")]
        public List<Task> Tasks { get; set; }

        [JsonProperty("skill_challenges")]
        public List<MappedModel> SkillChallenges { get; set; }

        [JsonProperty("sectors")]
        public List<Sector> Sectors { get; set; }
    }
}
