using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
using System.IO;
using System.Windows.Markup;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public static class ResourceUtility
    {
        public static BitmapImage LoadBitmapImage(string resourceUri)
        {
#if SILVERLIGHT
            return new BitmapImage(new Uri(string.Format("/GuildWars2.ArenaNet.Mapper.Silverlight;component{0}", resourceUri), UriKind.Relative));
#else
            return new BitmapImage(new Uri(string.Format("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component{0}", resourceUri)));
#endif
        }

        public static ControlTemplate LoadControlTemplate(string resourceUri)
        {
            ControlTemplate template;

#if SILVERLIGHT
            using (StreamReader sr = new StreamReader(Application.GetResourceStream(
                    new Uri(string.Format("/GuildWars2.ArenaNet.Mapper.Silverlight;component{0}", resourceUri), UriKind.Relative)).Stream))
            {
                template = (ControlTemplate)XamlReader.Load(sr.ReadToEnd()
                        .Replace(
                            "clr-namespace:GuildWars2.ArenaNet.Mapper;assembly=GuildWars2.ArenaNet.Mapper",
                            "clr-namespace:GuildWars2.ArenaNet.Mapper;assembly=GuildWars2.ArenaNet.Mapper.Silverlight")
                        .Replace(
                            "/GuildWars2.ArenaNet.Mapper;component",
                            "/GuildWars2.ArenaNet.Mapper.Silverlight;component"));
            }
#else
            template = (ControlTemplate)Application.LoadComponent(
                new Uri(string.Format("/GuildWars2.ArenaNet.Mapper;component{0}", resourceUri), UriKind.Relative));
#endif

            return template;
        }
    }
}
