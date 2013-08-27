using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#if SILVERLIGHT
using Color = System.Windows.Media.Color;

using Microsoft.Maps.MapControl;
using Location = Microsoft.Maps.MapControl.Location;
#else
using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;
#endif

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventMapPolygon : MapPolygon
    {
#if SILVERLIGHT
        private static SolidColorBrush REGULAR_EVENT_BRUSH_FILL = new SolidColorBrush(Colors.Blue);
        private static SolidColorBrush REGULAR_EVENT_BRUSH_STROKE = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x8B));
        private static SolidColorBrush CHAMPION_EVENT_BRUSH_FILL = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush CHAMPION_EVENT_BRUSH_STROKE = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x00, 0x00));
#else
        private static SolidColorBrush REGULAR_EVENT_BRUSH_FILL = Brushes.Blue;
        private static SolidColorBrush REGULAR_EVENT_BRUSH_STROKE = Brushes.DarkBlue;
        private static SolidColorBrush CHAMPION_EVENT_BRUSH_FILL = Brushes.Red;
        private static SolidColorBrush CHAMPION_EVENT_BRUSH_STROKE = Brushes.Maroon;
#endif

        public EventMapPolygon(EventDetails ev, bool hasChampion = false)
        {
            Locations = new LocationCollection();
            Opacity = 0.5;
            StrokeThickness = 2;
            if (hasChampion)
            {
                Fill = REGULAR_EVENT_BRUSH_FILL;
                Stroke = REGULAR_EVENT_BRUSH_STROKE;
            }
            else
            {
                Fill = CHAMPION_EVENT_BRUSH_FILL;
                Stroke = CHAMPION_EVENT_BRUSH_STROKE;
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
                    Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
