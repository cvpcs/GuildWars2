using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using GuildWars2.SyntaxError.Model;

#if !SILVERLIGHT
using System.Diagnostics;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public class PopupContentFactory
    {
        private static BitmapImage COPY_ICON = ResourceUtility.LoadBitmapImage("/Resources/copy_icon.png");

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

        public StackPanel Content { get; set; }

        public PopupContentFactory()
        {
            Content = new StackPanel();
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

            AppendParagraph(p);

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
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            TextBlock text = new TextBlock();
            text.Foreground = Brushes.Black;
            text.Inlines.Add(string.Format("Chat code: {0}", code.ToString()));
            text.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(text);

            Button copy = new Button();
            copy.Template = ResourceUtility.LoadControlTemplate("/ImageButtonTemplate.xaml");
            copy.Content = COPY_ICON;
            copy.Width = 18;
            copy.Height = 18;
            copy.Margin = new Thickness(2, 1, 0, 1);
            copy.Click += (s, e) => Clipboard.SetText(code.ToString());
#if SILVERLIGHT
            ToolTipService.SetToolTip(copy, "Copy to clipboard");
#else
            copy.ToolTip = "Copy to clipboard";
#endif

            panel.Children.Add(copy);

            Content.Children.Add(panel);

            return this;
        }

        private void AppendParagraph(Paragraph p)
        {
#if SILVERLIGHT
            RichTextBlock tb = new RichTextBlock();
#else
            TextBlock tb = new TextBlock();
#endif
            tb.Foreground = Brushes.Black;

#if SILVERLIGHT
            tb.Blocks.Add(p);
#else
            while (p.Inlines.Count > 0)
                tb.Inlines.Add(p.Inlines.FirstInline);
#endif

            Content.Children.Add(tb);
        }
    }
}
