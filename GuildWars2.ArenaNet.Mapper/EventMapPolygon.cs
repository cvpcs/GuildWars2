using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventMapPolygon : MapPolygon
    {
        private static SolidColorBrush FILL_BRUSH = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush STROKE_BRUSH = new SolidColorBrush(Colors.Maroon);

        public EventMapPolygon(EventDetails ev)
        {
            Locations = new LocationCollection();
            Opacity = 0.5;
            StrokeThickness = 2;
            Fill = FILL_BRUSH;
            Stroke = STROKE_BRUSH;
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
