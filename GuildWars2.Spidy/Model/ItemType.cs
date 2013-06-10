using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Spidy.Model
{
    public class ItemType : NamedModel
    {
        public List<ItemSubType> SubTypes { get; set; }
    }
}
