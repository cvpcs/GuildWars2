using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class Location
    {
        public string Type { get; set; }
        public LocationType TypeEnum
        {
            get
            {
                LocationType type;
                if (Enum.TryParse<LocationType>(Type, true, out type))
                    return type;

                return LocationType.Invalid;
            }
        }

        public List<double> Center { get; set; }

        #region Sphere / Cylinder
        public double Radius { get; set; }

        public double Rotation { get; set; }

        #region Cylinder
        public double Height { get; set; }
        #endregion
        #endregion

        #region Poly
        public List<double> ZRange { get; set; }

        public List<List<double>> Points { get; set; }
        #endregion
    }
}
