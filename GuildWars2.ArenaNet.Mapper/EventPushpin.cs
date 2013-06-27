using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class EventPushpin : ImagePushpin
    {
        private static IDictionary<EventFlagType, IList<BitmapImage>> IMAGES;

        static EventPushpin()
        {
            IMAGES = new Dictionary<EventFlagType, IList<BitmapImage>>();

            IMAGES.Add(EventFlagType.None, new List<BitmapImage>());
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/event_star_gray.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES[EventFlagType.None].Add(img);
            img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/event_star.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES[EventFlagType.None].Add(img);

            IMAGES.Add(EventFlagType.GroupEvent, new List<BitmapImage>());
            img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/event_boss_gray.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES[EventFlagType.GroupEvent].Add(img);
            img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = Application.GetResourceStream(new Uri("/Resources/event_boss.png", UriKind.Relative)).Stream;
            img.EndInit();
            IMAGES[EventFlagType.GroupEvent].Add(img);
        }

        private BitmapImage m_PreparationImage;
        private BitmapImage m_ActiveImage;

        public EventPushpin(EventDetails ev)
            : base()
        {
            Width = 20;
            Height = 20;

            PositionOrigin = PositionOrigin.Center;

            if (!string.IsNullOrWhiteSpace(ev.Name))
                ToolTip = ev.Name;

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
                    ToolTip = "[Group Event] " + ev.Name;
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
                    Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
