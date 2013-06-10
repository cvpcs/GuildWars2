using System;
using System.Collections.Generic;

using GuildWars2.TradingPost.API;

namespace GuildWars2.TradingPost.ListingsToExcel
{
    public static class MeRequestExtensions
    {
        public static List<MeResponse> GetNew(this MeRequest request, int currentTotal, int count = 10)
        {
            List<MeResponse> result = new List<MeResponse>();

            int old_offset = request.Offset;
            int old_count = request.Count;
            request.Count = count;

            request.Offset = 1;

            MeResponse response = request.Execute();

            if (response != null)
            {
                int total = response.Total;
                int numNew = total - currentTotal;

                for (request.Offset = 1; request.Offset + request.Count <= numNew; request.Offset += request.Count)
                {
                    response = request.Execute();
                    if (response != null)
                        result.Add(response);
                }

                request.Count = numNew - (request.Offset - 1);
                response = request.Execute();
                if (response != null)
                    result.Add(response);
            }

            request.Offset = old_offset;
            request.Count = old_count;

            return result;
        }
    }
}
