using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.SyntaxError.API
{
    public partial class ChampionEventsRequest
    {
        public IList<Guid> Execute()
        {
            string html;

            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(URL);
            }

            List<string> list = ParseChampionWikiPage(html);

            EventNamesResponse names = new EventNamesRequest(LanguageCode.EN).Execute();

            return names.Where(ev => list.Contains(NameClean.Replace(ev.Name.ToLower(), string.Empty))).Select(ev => ev.Id).ToList();
        }
    }
}
