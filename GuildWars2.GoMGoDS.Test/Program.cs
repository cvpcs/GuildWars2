using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.GoMGoDS.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime time = new DateTime(2012, 3, 5, 20, 3, 6);
            int secs = 60;

            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 7;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 30;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 2 * 60 * 60;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 60 * 60;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 30 * 60;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 24 * 60 * 60;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            secs = 3 * 24 * 60 * 60;
            Console.WriteLine("Round to nearest {0} seconds: {1}", secs, time.Round(new TimeSpan(0, 0, secs)));
            Console.ReadKey(true);

        }
    }
}
