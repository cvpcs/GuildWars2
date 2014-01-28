using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public abstract class TemplatedPushpin : Pushpin
    {
        private static IDictionary<string, ControlTemplate> TEMPLATES = new Dictionary<string, ControlTemplate>();

        public TemplatedPushpin(string templateResourceUri)
            : base()
        {
            if (!TEMPLATES.ContainsKey(templateResourceUri))
                TEMPLATES[templateResourceUri] = ResourceUtility.LoadControlTemplate(templateResourceUri);

            PositionOrigin = PositionOrigin.Center;

            Template = TEMPLATES[templateResourceUri];
        }
    }
}
