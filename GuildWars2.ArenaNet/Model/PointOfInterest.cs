using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class PointOfInterest : MappedModel
    {
        public int PoiId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
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

        public int Floor { get; set; }
    }
}
