using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class PointOfInterest : MappedModel
    {
        [JsonProperty("poi_id")]
        public int PoiId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
        public PointOfInterestType TypeEnum
        {
            get
            {
                PointOfInterestType type;
                if (Enum.TryParse<PointOfInterestType>(Type, true, out type))
                    return type;

                return PointOfInterestType.Invalid;
            }
        }

        [JsonProperty("floor")]
        public int Floor { get; set; }
    }
}
