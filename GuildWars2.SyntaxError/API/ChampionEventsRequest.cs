using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.SyntaxError.API
{
    public class ChampionEventsRequest
    {
        private Regex NameClean = new Regex("[^a-z0-9]", RegexOptions.Compiled);

        public IList<Guid> Execute()
        {
            string html;
            List<string> list = new List<string>();

            using (WebClient client = new WebClient())
            {
                html = client.DownloadString("http://wiki.guildwars2.com/wiki/List_of_champions");
            }

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(html);
            sw.Flush();
            ms.Position = 0;
            XPathDocument docNav = new XPathDocument(ms);
            sw.Close();

            XPathNavigator nav = docNav.CreateNavigator();

            XPathNodeIterator i = nav.Select("//table/tr/td[position()=6]");

            while (i.MoveNext())
            {
                XPathNavigator node = i.Current.SelectSingleNode("./a");
                
                if (node != null)
                    list.Add(NameClean.Replace(node.Value.ToLower(), string.Empty));
            }

            EventNamesResponse names = new EventNamesRequest(LanguageCode.EN).Execute();

            return names.Where(ev => list.Contains(NameClean.Replace(ev.Name.ToLower(), string.Empty))).Select(ev => ev.Id).ToList();
        }
    }
}
