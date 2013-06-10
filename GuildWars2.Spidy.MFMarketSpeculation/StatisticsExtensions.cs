using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GuildWars2.Spidy.MFMarketSpeculation
{
    public static class StatisticsExtensions
    {
        public static double InterquartileMean(this IEnumerable<int> data)
        {
            List<int> list = data.ToList();
            list.Sort();

            int length = list.Count;
            int quartile = (int)Math.Floor((double)length / 4.0);
            double fraction = 1.0 - (((double)length / 4.0) - quartile);

            // remove our quartiles
            list.RemoveRange(length - quartile, quartile);
            list.RemoveRange(0, quartile);

            double partial = (double)(list[0] + list[list.Count - 1]) * fraction;

            // remove partials
            list.RemoveAt(list.Count - 1);
            list.RemoveAt(0);

            return (partial + (double)list.Sum()) / ((double)length / 2.0);
        }
    }
}
