using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp.Deserializers;

namespace GuildWars2.Spidy.Model
{
    public abstract class NamedModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
