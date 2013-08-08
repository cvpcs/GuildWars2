using System;
using System.Collections.Generic;

using GuildWars2.SyntaxError.API;

namespace GuildWars2.SyntaxError.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IList<Guid> list = new ChampionEventsRequest().Execute();

            int i = list.Count;
        }
    }
}
