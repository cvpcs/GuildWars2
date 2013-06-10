using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;

namespace GuildWars2.Spidy.API
{
    public abstract class PaginatedRequest<T> : Request<T>
        where T : PaginatedResponse, new()
    {
        public int Page { get; set; }

        public PaginatedRequest(int page = 1)
        {
            Page = page;
        }

        public List<T> ExecuteAllPages()
        {
            int old_page = Page;
            List<T> result = new List<T>();

            Page = 1;
            T page = Execute();

            if (page != null)
            {
                result.Add(page);

                int last_page = page.LastPage;
                for (Page = 2; Page <= last_page; Page++)
                {
                    page = Execute();
                    if (page != null)
                    {
                        result.Add(page);
                    }
                }
            }

            Page = old_page;
            return result;
        }
    }
}
