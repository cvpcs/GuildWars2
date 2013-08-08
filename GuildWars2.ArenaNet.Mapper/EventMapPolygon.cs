using System;
using System.Windows;
using System.Windows.Media;

using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventMapPolygon : MapPolygon
    {
        public EventMapPolygon(EventDetails ev, bool hasChampion = false)
        {
            Locations = new LocationCollection();
            Opacity = 0.5;
            StrokeThickness = 2;
            if (hasChampion)
            {
                Fill = Brushes.Blue;
                Stroke = Brushes.DarkBlue;
            }
            else
            {
                Fill = Brushes.Red;
                Stroke = Brushes.Maroon;
            }
        }

        public void SetEventState(EventStateType state)
        {
            switch (state)
            {
                case EventStateType.Active:
                    Visibility = Visibility.Visible;
                    break;
                default:
                    Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
