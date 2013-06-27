using System;
using System.Windows.Controls;

using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventMapLayer : MapLayer
    {
        private EventPushpin m_Pushpin;
        public Location Center
        {
            get { return m_Pushpin.Location; }
            set { m_Pushpin.Location = value; }
        }

        public EventMapLayer(EventDetails ev)
        {
            m_Pushpin = new EventPushpin(ev);
            Children.Add(m_Pushpin);
        }

        public virtual void SetEventState(EventStateType state)
        {
            m_Pushpin.SetEventState(state);
        }
    }
}
