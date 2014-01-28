using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class FloorMapDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("min_level")]
        public int MinLevel { get; set; }

        [JsonProperty("max_level")]
        public int MaxLevel { get; set; }

        [JsonProperty("default_floor")]
        public int DefaultFloor { get; set; }

        [JsonProperty("map_rect")]
        public List<List<double>> MapRect { get; set; }

        [JsonProperty("continent_rect")]
        public List<List<double>> ContinentRect { get; set; }

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
