using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.API
{
    public abstract class CountedRequest<T> : Request<T>
        where T : CountedResponse, new()
    {
        public int Count { get; set; }
        public int Offset { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["count"] = Count.ToString();
                parameters["offset"] = Offset.ToString();

                return parameters;
            }
        }

        public CountedRequest(int count = 10, int offset = 1)
        {
            Count = count;
            Offset = offset;
        }

        public List<T> ExecuteAll()
        {
            List<T> result = new List<T>();

            int old_offset = Offset;
            Offset = 1;

            T page = Execute();

            if (page != null)
            {
                result.Add(page);

                int total = page.Total;
                for (Offset += Count; Offset <= total; Offset += Count)
                {
                    page = Execute();
                    if (page != null)
                    {
                        result.Add(page);
                    }
                }
            }

            Offset = old_offset;
            return result;
        }
    }
}
