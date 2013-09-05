using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#if SILVERLIGHT
using System.IO;
using System.Windows.Markup;

using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public abstract class TemplatedPushpin : Pushpin
    {
        private static IDictionary<string, ControlTemplate> TEMPLATES = new Dictionary<string, ControlTemplate>();

        private static ControlTemplate LoadTemplateResource(string resourceUri)
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

        public TemplatedPushpin(string templateResourceUri)
            : base()
        {
            if (!TEMPLATES.ContainsKey(templateResourceUri))
                TEMPLATES[templateResourceUri] = LoadTemplateResource(templateResourceUri);

            PositionOrigin = PositionOrigin.Center;

            Template = TEMPLATES[templateResourceUri];
        }
    }
}
