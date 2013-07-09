using System;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class SectorPushpin : Pushpin
    {
        public string SectorName { get; private set; }
        public string SectorLevel { get; private set; }

        public SectorPushpin(Sector sector)
            : base()
        {
            PositionOrigin = PositionOrigin.Center;

            Width = Double.NaN;
            Height = Double.NaN;

            SectorName = sector.Name;
            SectorLevel = string.Format("Level {0}", sector.Level);
        }
    }
}
