using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventPolyMapLayer : EventMapLayer
    {
        private static SolidColorBrush FILL_BRUSH = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush STROKE_BRUSH = new SolidColorBrush(Colors.Maroon);

        private MapPolygon m_MapPolygon;
        public LocationCollection PolyLocations
        {
            get { return m_MapPolygon.Locations; }
            set { m_MapPolygon.Locations = value; }
        }

        public EventPolyMapLayer(EventDetails ev)
            : base(ev)
        {
            m_MapPolygon = new MapPolygon();
            m_MapPolygon.Locations = new LocationCollection();
            m_MapPolygon.Opacity = 0.5;
            m_MapPolygon.StrokeThickness = 2;
            m_MapPolygon.Fill = FILL_BRUSH;
            m_MapPolygon.Stroke = STROKE_BRUSH;
            
            // insert so the polygon will always be under the event pushpin
            Children.Insert(0, m_MapPolygon);
        }

        public override void SetEventState(EventStateType state)
        {
            base.SetEventState(state);

            switch (state)
            {
                case EventStateType.Active:
                    m_MapPolygon.Visibility = Visibility.Visible;
                    break;
                default:
                    m_MapPolygon.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
