using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Trader.Model
{
    public class ItemData
    {
        public int Sell { get; set; }

        public int Buy { get; set; }

        public long Updated { get; set; }

        public DateTime UpdatedDateTime
        {
            get
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds(Updated);
            }
        }
    }
}
