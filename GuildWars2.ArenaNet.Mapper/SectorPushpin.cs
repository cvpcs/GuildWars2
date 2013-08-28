using System;
using System.Windows;
using System.Windows.Controls;

#if SILVERLIGHT
using System.IO;
using System.Windows.Markup;

using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class SectorPushpin : Pushpin
    {
        private static ControlTemplate TEMPLATE;

        static SectorPushpin()
        {
#if SILVERLIGHT
            using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri("/GuildWars2.ArenaNet.Mapper;component/SectorPushpinTemplate.xaml", UriKind.Relative)).Stream))
            {
                TEMPLATE = (ControlTemplate)XamlReader.Load(sr.ReadToEnd());
            }
#else
            TEMPLATE = (ControlTemplate)Application.LoadComponent(
                new Uri("/GuildWars2.ArenaNet.Mapper;component/SectorPushpinTemplate.xaml", UriKind.Relative));
#endif
        }

        public string SectorName { get; private set; }
        public string SectorLevel { get; private set; }

        public SectorPushpin(Sector sector)
            : base()
        {
            PositionOrigin = PositionOrigin.Center;

            Width = Double.NaN;
            Height = Double.NaN;

            Template = TEMPLATE;

            SectorName = sector.Name;
            SectorLevel = string.Format("Level {0}", sector.Level);
        }
    }
}
