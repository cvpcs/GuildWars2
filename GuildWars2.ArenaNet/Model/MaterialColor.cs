using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class MaterialColor
    {
        public int Brightness { get; set; }

        public double Contrast { get; set; }

        public int Hue { get; set; }

        public double Saturation { get; set; }

        public double Lightness { get; set; }

        public List<int> Rgb { get; set; }
    }
}
