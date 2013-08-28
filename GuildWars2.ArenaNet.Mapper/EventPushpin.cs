using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventPushpin : ImagePushpin
    {
        private static IDictionary<EventFlagType, IList<BitmapImage>> IMAGES = new Dictionary<EventFlagType, IList<BitmapImage>>() {
                { EventFlagType.None, new List<BitmapImage>() {
                        new BitmapImage(new Uri("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component/Resources/event_star_gray.png")),
                        new BitmapImage(new Uri("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component/Resources/event_star.png"))
                    } },
                { EventFlagType.GroupEvent, new List<BitmapImage>() {
                        new BitmapImage(new Uri("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component/Resources/event_boss_gray.png")),
                        new BitmapImage(new Uri("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component/Resources/event_boss.png"))
                    } }
            };

        private BitmapImage m_PreparationImage;
        private BitmapImage m_ActiveImage;

        public EventPushpin(EventDetails ev)
            : base()
        {
            if (!string.IsNullOrWhiteSpace(ev.Name))
                ToolTip = string.Format("{0} ({1})", ev.Name, ev.Level);

            if (ev.FlagsEnum == EventFlagType.None)
            {
                m_PreparationImage = IMAGES[EventFlagType.None][0];
                m_ActiveImage = IMAGES[EventFlagType.None][1];
            }
            else if ((ev.FlagsEnum & EventFlagType.GroupEvent) == EventFlagType.GroupEvent)
            {
                m_PreparationImage = IMAGES[EventFlagType.GroupEvent][0];
                m_ActiveImage = IMAGES[EventFlagType.GroupEvent][1];

                if (!string.IsNullOrWhiteSpace(ev.Name))
                    ToolTip = string.Format("[Group Event] {0} ({1})", ev.Name, ev.Level);
            }

            SetEventState(EventStateType.Invalid);
        }

        public void SetEventState(EventStateType state)
        {
            switch (state)
            {
                case EventStateType.Active:
                    Visibility = Visibility.Visible;
                    Image = m_ActiveImage;
                    break;
                case EventStateType.Preparation:
                    Visibility = Visibility.Visible;
                    Image = m_PreparationImage;
                    break;
                default:
                    Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
