using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using GuildWars2.SyntaxError.Model;

#if !SILVERLIGHT
using System.Diagnostics;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public class PopupContentFactory
    {
        private static IDictionary<string, Uri> DULFY_BOUNTY_LINKS = new Dictionary<string, Uri>()
            {
                { "2-MULT", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#0") },
                { "Ander \"Wildman\" Westward", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#1") },
                { "Big Mayana", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#1b") },
                { "Bookworm Bwikki", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#2") },
                { "Brekkabek", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#3") },
                { "Crusader Michiele", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#4") },
                { "\"Deputy\" Brooke", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#5") },
                { "Devious Teesa", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#6") },
                { "Diplomat Tarban", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#7") },
                { "Half-Baked Komali", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#8") },
                { "Poobadoo", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#9") },
                { "Prisoner 1141", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#10") },
                { "Shaman Arderus", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#11") },
                { "Short-Fuse Felix", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#12") },
                { "Sotzz the Scallywag", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#13") },
                { "Tricksy Trekksa", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#14") },
                { "Trillia Midwell", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#15") },
                { "Yanonka the Rat-Wrangler", new Uri("http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#16") },
            };

        private IList<Paragraph> m_Paragraphs;

        public PopupContentFactory()
        {
            m_Paragraphs = new List<Paragraph>();
        }

        public PopupContentFactory AppendLink(string label, string text, Uri uri, string target = "_blank")
        {
            Paragraph p = new Paragraph();
            p.Inlines.Add(string.Format("{0}: ", label));

            Hyperlink link = new Hyperlink();
            link.NavigateUri = uri;
            link.TargetName = target;
#if !SILVERLIGHT
            link.RequestNavigate += (s, e) => Process.Start(e.Uri.AbsoluteUri);
#endif
            link.Inlines.Add(text);

            p.Inlines.Add(link);

            m_Paragraphs.Add(p);

            return this;
        }

        public PopupContentFactory AppendText(string label, string text)
        {
            Paragraph p = new Paragraph();

            p.Inlines.Add(string.Format("{0}: {1}", label, text));

            m_Paragraphs.Add(p);

            return this;
        }

        public PopupContentFactory AppendDulfyLink(string bountyName)
        {
            if (DULFY_BOUNTY_LINKS.ContainsKey(bountyName))
                AppendLink("Dulfy page", bountyName, DULFY_BOUNTY_LINKS[bountyName]);

            return this;
        }

        public PopupContentFactory AppendWikiLink(string article)
        {
            // trim puncutation at the end, the wiki doesn't expect it
            string articleTrimmed = article.TrimEnd('.', '!', '?');

            return AppendLink("Wiki page", articleTrimmed, new Uri(string.Format("http://wiki.guildwars2.com/wiki/Special:Search/{0}", articleTrimmed)));
        }

        public PopupContentFactory AppendChatCode(ChatCode code)
        {
            return AppendText("Chat code", code.ToString());
        }

        public UIElement GetContent()
        {
#if SILVERLIGHT
            RichTextBlock tb = new RichTextBlock();
#else
            TextBlock tb = new TextBlock();
#endif
            tb.Foreground = Brushes.Black;

            for (int i = 0; i < m_Paragraphs.Count; i++)
            {
                Paragraph p = m_Paragraphs[i];
#if SILVERLIGHT
                tb.Blocks.Add(p);
#else
                while (p.Inlines.Count > 0)
                    tb.Inlines.Add(p.Inlines.FirstInline);
                if (i < m_Paragraphs.Count - 1)
                    tb.Inlines.Add(new LineBreak());
#endif
            }

            return tb;
        }
    }
}
