using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

using GuildWars2.TradingPost;
using HtmlAgilityPack;
using RestSharp;

namespace GuildWars2.TradingPost.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string html;
            SessionManager sm = SessionManager.GetInstance();
            sm.Email = ""; // set email before running
            sm.Password = ""; // set password before running

            RequestUtil request = RequestUtil.NewInstance("https://leaderboards.guildwars2.com", "/en/na/achievements/guild/Guíld%20Of%20Dívíne%20Soldíers");
            request.SetMethod(Method.GET);
            request.SetCookie("s", sm.Session.Key);
            IRestResponse r = request.Execute();
            html = r.Content;

            List<user> l = ParseUserPage(html);

            TextArea.FontFamily = new FontFamily("Courier New");
            TextArea.Text = string.Format("{0,-20} | {1,-6} | {2,-20} | {3}\n{4}", "User", "AP", "World", "Last Earned AP",
                string.Join("\n", l.Select(u => string.Format("{0,-20} | {1,6} | {2,-20} | {3}", u.account, u.ap, u.world, u.lastapgain))));
        }

        private List<user> ParseUserPage(string html)
        {
            List<user> list = new List<user>();

            HtmlDocument docNav = new HtmlDocument();
            docNav.LoadHtml(html);

            XPathNavigator nav = docNav.CreateNavigator();

            XPathNodeIterator i = nav.Select("//table[contains(@class, 'achievements') and contains(@class, 'real')]/tbody/tr");

            while (i.MoveNext())
            {
                user u = new user();

                XPathNavigator node = i.Current.SelectSingleNode("./td[contains(@class, 'name')]");
                if (node != null) u.account = node.Value.Trim();
                
                node = i.Current.SelectSingleNode("./td[contains(@class, 'achievements')]/span/span[not(contains(@class, 'additional'))]");
                if (node != null) int.TryParse(node.Value.Trim(), out u.ap);
                
                node = i.Current.SelectSingleNode("./td[contains(@class, 'achievements')]/span/span[contains(@class, 'additional')]");
                if (node != null) u.lastapgain = node.Value.Remove(0, node.Value.IndexOf("Since") + 5).Replace("&#x2F;", "/").Trim();
                
                node = i.Current.SelectSingleNode("./td[contains(@class, 'world')]");
                if (node != null) u.world = node.Value.Trim();
                
                list.Add(u);
            }

            return list;
        }

        private struct user
        {
            public string account;
            public int ap;
            public string lastapgain;
            public string world;
        }
    }
}
