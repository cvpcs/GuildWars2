using System;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class SectorPushpin : TemplatedPushpin
    {
        public string SectorName { get; private set; }
        public string SectorLevel { get; private set; }

        public SectorPushpin(Sector sector)
            : base("/SectorPushpinTemplate.xaml")
        {
            PositionOrigin = PositionOrigin.Center;

            Width = Double.NaN;
            Height = Double.NaN;

            SectorName = sector.Name;
            SectorLevel = string.Format("Level {0}", sector.Level);
        }
    }
}
