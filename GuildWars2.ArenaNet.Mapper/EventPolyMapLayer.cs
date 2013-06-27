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
        private static SolidColorBrush ACTIVE_FILL_BRUSH = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush ACTIVE_STROKE_BRUSH = new SolidColorBrush(Colors.Maroon);
        private static SolidColorBrush PREPARATION_FILL_BRUSH = new SolidColorBrush(Colors.Gray);
        private static SolidColorBrush PREPARATION_STROKE_BRUSH = new SolidColorBrush(Colors.DarkGray);

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
                    m_MapPolygon.Stroke = ACTIVE_STROKE_BRUSH;
                    m_MapPolygon.Fill = ACTIVE_FILL_BRUSH;
                    break;
                case EventStateType.Preparation:
                    m_MapPolygon.Visibility = Visibility.Visible;
                    m_MapPolygon.Stroke = PREPARATION_STROKE_BRUSH;
                    m_MapPolygon.Fill = PREPARATION_FILL_BRUSH;
                    break;
                default:
                    m_MapPolygon.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
