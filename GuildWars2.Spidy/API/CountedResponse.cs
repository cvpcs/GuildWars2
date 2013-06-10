using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Spidy.API
{
    public abstract class CountedResponse
    {
        public int Count { get; set; }
    }
}
