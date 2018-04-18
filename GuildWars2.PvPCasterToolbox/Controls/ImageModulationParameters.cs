using System;

namespace GuildWars2.PvPCasterToolbox.Controls
{
    public class ImageModulationParameters : IEquatable<ImageModulationParameters>
    {
        public static readonly int HueMinValue = -180;
        public static readonly int HueMaxValue = 180;

        public static readonly int SaturationMinValue = -100;
        public static readonly int SaturationMaxValue = 100;

        public static readonly int BrightnessMinValue = -100;
        public static readonly int BrightnessMaxValue = 100;

        public int Hue = 0;
        public int Saturation = 0;
        public int Brightness = 0;

        public double HuePercentage => ValueToPercentage(Hue, HueMinValue, HueMaxValue);
        public double SaturationPercentage => ValueToPercentage(Saturation, SaturationMinValue, SaturationMaxValue);
        public double BrightnessPercentage => ValueToPercentage(Brightness, BrightnessMinValue, BrightnessMaxValue);

        public ImageModulationParameters(int hue, int saturation, int brightness)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
        }

        public bool Equals(ImageModulationParameters other)
            => Hue == other?.Hue &&
               Saturation == other?.Saturation &&
               Brightness == other?.Brightness;

        public override bool Equals(object obj)
            => !(obj is null) &&
               (ReferenceEquals(this, obj) || Equals(obj as ImageModulationParameters));

        public override int GetHashCode()
            => (Hue, Saturation, Brightness).GetHashCode();

        private static double ValueToPercentage(int val, int min, int max)
            => 200 * ((double)(val - min) / (double)(max - min));
    }
}
