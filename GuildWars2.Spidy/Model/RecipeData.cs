using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.Spidy.Model
{
    public class RecipeData
    {
        public int DataId { get; set; }

        public string Name { get; set; }

        public int ResultCount { get; set; }

        public int ResultItemDataId { get; set; }

        public int DisciplineId { get; set; }

        public int ResultItemMaxOfferUnitPrice { get; set; }

        public int ResultItemMinSaleUnitPrice { get; set; }

        public int CraftingCost { get; set; }

        public int Rating { get; set; }
    }
}
