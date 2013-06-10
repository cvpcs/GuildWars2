using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp.Deserializers;

namespace GuildWars2.Spidy.Model
{
    public class ItemListing
    {
        [DeserializeAs(Name="listing_datetime")]
        public DateTime ListingDateTime { get; set; }

        public int UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int Listings { get; set; }
    }
}
