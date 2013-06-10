using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuildWars2.Spidy.Model;

namespace GuildWars2.Spidy.API
{
    public class RecipeListRequest : PaginatedRequest<RecipeListResponse>
    {
        public Discipline Discipline { get; set; }

        public RecipeListRequest(Discipline discipline = null, int page = 1)
            : base(page)
        {
            Discipline = discipline;
        }

        public RecipeListRequest(int page, Discipline discipline = null)
            : this(discipline, page)
        {
        }

        protected override string GetAPIPath()
        {
            return "/api/" + VERSION + "/" + FORMAT + "/recipes/" + (Discipline == null ? "*all*" : Discipline.Id.ToString()) + "/" + Page;
        }
    }
}
