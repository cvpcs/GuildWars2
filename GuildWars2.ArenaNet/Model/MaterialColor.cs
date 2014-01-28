using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class MaterialColor
    {
        [JsonProperty("brightness")]
        public int Brightness { get; set; }

        [JsonProperty("contrast")]
        public double Contrast { get; set; }

        [JsonProperty("hue")]
        public int Hue { get; set; }

        [JsonProperty("saturation")]
        public double Saturation { get; set; }

        [JsonProperty("lightness")]
        public double Lightness { get; set; }

        [JsonProperty("rgb")]
        public List<int> Rgb { get; set; }
    }
}
