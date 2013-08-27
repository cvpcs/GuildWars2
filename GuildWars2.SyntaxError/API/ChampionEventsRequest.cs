using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GuildWars2.SyntaxError.API
{
    public partial class ChampionEventsRequest
    {
        private static Regex NameClean = new Regex("[^a-z0-9]");
        private static Uri URL = new Uri("http://wiki.guildwars2.com/wiki/List_of_champions");

        private List<string> ParseChampionWikiPage(string html)
        {
            List<string> list = new List<string>();

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(html);
            sw.Flush();
            ms.Position = 0;
            XDocument docNav = XDocument.Load(ms);
            sw.Close();

            XPathNavigator nav = docNav.CreateNavigator();

            XPathNodeIterator i = nav.Select("//table/tr/td[position()=6]");

            while (i.MoveNext())
            {
                XPathNavigator node = i.Current.SelectSingleNode("./a");

                if (node != null)
                    list.Add(NameClean.Replace(node.Value.ToLower(), string.Empty));
            }

            return list;
        }
    }
}
