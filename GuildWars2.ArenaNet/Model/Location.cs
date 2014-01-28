using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class Location
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
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

        [JsonProperty("center")]
        public List<double> Center { get; set; }

        #region Sphere / Cylinder
        [JsonProperty("radius")]
        public double Radius { get; set; }
        public bool ShouldSerializeRadius() { return TypeEnum == LocationType.Sphere || TypeEnum == LocationType.Cylinder; }

        [JsonProperty("rotation")]
        public double Rotation { get; set; }
        public bool ShouldSerializeRotation() { return TypeEnum == LocationType.Sphere || TypeEnum == LocationType.Cylinder; }

        #region Cylinder
        [JsonProperty("height")]
        public double Height { get; set; }
        public bool ShouldSerializeHeight() { return TypeEnum == LocationType.Cylinder; }
        #endregion
        #endregion

        #region Poly
        [JsonProperty("z_range")]
        public List<double> ZRange { get; set; }
        public bool ShouldSerializeZRange() { return TypeEnum == LocationType.Poly; }

        [JsonProperty("points")]
        public List<List<double>> Points { get; set; }
        public bool ShouldSerializePoints() { return TypeEnum == LocationType.Poly; }
        #endregion
    }
}
