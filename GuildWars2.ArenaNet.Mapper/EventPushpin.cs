using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventPushpin : ImagePushpin
    {
        private static IDictionary<EventFlagType, IList<BitmapImage>> IMAGES = new Dictionary<EventFlagType, IList<BitmapImage>>() {
                { EventFlagType.None, new List<BitmapImage>() {
                        ResourceUtility.LoadBitmapImage("/Resources/event_star_gray.png"),
                        ResourceUtility.LoadBitmapImage("/Resources/event_star.png")
                    } },
                { EventFlagType.GroupEvent, new List<BitmapImage>() {
                        ResourceUtility.LoadBitmapImage("/Resources/event_boss_gray.png"),
                        ResourceUtility.LoadBitmapImage("/Resources/event_boss.png")
                    } }
            };

        private BitmapImage m_PreparationImage;
        private BitmapImage m_ActiveImage;

        public EventPushpin(EventDetails ev)
            : base()
        {
            if (!string.IsNullOrWhiteSpace(ev.Name))
            {
                ToolTip = string.Format("{0}{1} ({2})",
                        ((ev.FlagsEnum & EventFlagType.GroupEvent) == EventFlagType.GroupEvent ? "[Group Event] " : string.Empty),
                        ev.Name, ev.Level);

                PopupContent = new PopupContentFactory()
                        .AppendWikiLink(ev.Name)
                        .GetContent();
            }

            if (ev.FlagsEnum == EventFlagType.None)
            {
                m_PreparationImage = IMAGES[EventFlagType.None][0];
                m_ActiveImage = IMAGES[EventFlagType.None][1];
            }
            else if ((ev.FlagsEnum & EventFlagType.GroupEvent) == EventFlagType.GroupEvent)
            {
                m_PreparationImage = IMAGES[EventFlagType.GroupEvent][0];
                m_ActiveImage = IMAGES[EventFlagType.GroupEvent][1];
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
