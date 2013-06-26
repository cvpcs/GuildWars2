using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class FloorMapDetails
    {
        public string Name { get; set; }

        public int MinLevel { get; set; }

        public int MaxLevel { get; set; }

        public int DefaultFloor { get; set; }

        public List<List<double>> MapRect { get; set; }

        public List<List<double>> ContinentRect { get; set; }
        
        public List<PointOfInterest> PointsOfInterest { get; set; }

        public List<Task> Tasks { get; set; }

        public List<MappedModel> SkillChallenges { get; set; }

        public List<Sector> Sectors { get; set; }
    }
}
