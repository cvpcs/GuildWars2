using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.SyntaxError.API
{
    public partial class ChampionEventsRequest
    {
        public void ExecuteAsync(Action<IList<Guid>> callback)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.BeginGetResponse(asyncResult => {
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                    string html;

                    using (StreamReader ss = new StreamReader(response.GetResponseStream()))
                    {
                        html = ss.ReadToEnd();
                    }
                    
                    List<string> list = ParseChampionWikiPage(html);

                    new EventNamesRequest(LanguageCode.EN).ExecuteAsync(names => {
                            callback(names.Where(ev => list.Contains(NameClean.Replace(ev.Name.ToLower(), string.Empty))).Select(ev => ev.Id).ToList());
                        });
                }, request);
        }
    }
}
