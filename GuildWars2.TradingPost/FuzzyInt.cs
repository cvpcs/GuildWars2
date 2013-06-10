using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.TradingPost
{
    public class FuzzyInt
    {
        private static Random m_RAND = new Random();

        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public int Value
        {
            get
            {
                return m_RAND.Next(Minimum, Maximum);
            }
            set
            {
                Minimum = value;
                Maximum = value;
            }
        }

        public FuzzyInt(int val = 0)
            : this(val, val)
        { }

        public FuzzyInt(int min, int max)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}
