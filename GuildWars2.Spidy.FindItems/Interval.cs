using System;
using System.Text.RegularExpressions;

namespace GuildWars2.Spidy.FindItems
{
    public class Interval
    {
        private static Regex PARSE_REGEX = new Regex(@"^\s*([\(\[])\s*(-?[0-9]+|-inf)\s*,\s*([0-9]+|inf)\s*([\)\]])\s*$", RegexOptions.Compiled);

        public int? Minimum;
        public int? Maximum;

        public Interval(string str)
        {
            Parse(str);
        }

        public void Parse(string str)
        {
            // default min/max to infinity
            Minimum = null;
            Maximum = null;

            Match match = PARSE_REGEX.Match(str);

            if (match.Success && match.Groups.Count == 5)
            {
                int val;
                if (match.Groups[2].Value != "-inf" && int.TryParse(match.Groups[2].Value, out val))
                    Minimum = val + (match.Groups[1].Value == "(" ? 1 : 0);

                if (match.Groups[3].Value != "inf" && int.TryParse(match.Groups[3].Value, out val))
                    Maximum = val - (match.Groups[4].Value == ")" ? 1 : 0);
            }
        }

        public bool Contains(int value)
        {
            if (Minimum.HasValue && value < Minimum)
                return false;
            if (Maximum.HasValue && value > Maximum)
                return false;

            return true;
        }

        public override string ToString()
        {
            return "(" +
                (Minimum.HasValue ? (Minimum.Value - 1) + "" : "-inf") + "," +
                (Maximum.HasValue ? (Maximum.Value + 1) + "" : "inf") + ")";
        }
    }
}
