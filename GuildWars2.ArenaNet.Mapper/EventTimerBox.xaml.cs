using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    /// <summary>
    /// Interaction logic for EventTimerBox.xaml
    /// </summary>
    public partial class EventTimerBox : UserControl
    {
        private static LinearGradientBrush BOSS_GRADIENT = new LinearGradientBrush(
                new GradientStopCollection() {
                        new GradientStop() { Color = Color.FromArgb(  0,   0,   0,   0), Offset =  0.0 },
                        new GradientStop() { Color = Color.FromArgb(128, 220,   0,   0), Offset =  0.1 },
                        new GradientStop() { Color = Color.FromArgb(192, 220,   0,   0), Offset =  0.5 },
                        new GradientStop() { Color = Color.FromArgb(128, 220,   0,   0), Offset =  0.9 },
                        new GradientStop() { Color = Color.FromArgb(  0,   0,   0,   0), Offset =  1.0 }
                    }, 0.0);
        private static LinearGradientBrush PRE_GRADIENT = new LinearGradientBrush(
                new GradientStopCollection() {
                        new GradientStop() { Color = Color.FromArgb(  0,   0,   0,   0), Offset =  0.0 },
                        new GradientStop() { Color = Color.FromArgb(128, 220, 220,   0), Offset =  0.1 },
                        new GradientStop() { Color = Color.FromArgb(192, 220, 220,   0), Offset =  0.5 },
                        new GradientStop() { Color = Color.FromArgb(128, 220, 220,   0), Offset =  0.9 },
                        new GradientStop() { Color = Color.FromArgb(  0,   0,   0,   0), Offset =  1.0 }
                    }, 0.0);

#if SILVERLIGHT
        private static SolidColorBrush WINDOW_FOREGROUND = new SolidColorBrush(Colors.Blue);
        private static SolidColorBrush BEHIND_FOREGROUND = new SolidColorBrush(Colors.Red);
#else
        private static SolidColorBrush WINDOW_FOREGROUND = Brushes.Blue;
        private static SolidColorBrush BEHIND_FOREGROUND = Brushes.Red;
#endif

        private bool m_Countdown;
        private TimeSpan m_EventTime;

        public EventTimerBox()
        {
            InitializeComponent();

            // default to being invisible
            Visibility = Visibility.Collapsed;
        }

        public void SetData(MetaEventStatus e)
        {
            EventName.Text = e.Name;

            DateTime then = new DateTime(1970, 1, 1).AddMilliseconds(e.Timestamp);
            TimeSpan span = DateTime.UtcNow - then;

            m_Countdown = false;

            string windowType = null;
            if (e.MinCountdown > 0)
            {
                m_Countdown = true;

                if (e.MinCountdown - span.TotalSeconds > 0)
                    span = new TimeSpan(0, 0, (int)e.MinCountdown) - span;
                else
                {
                    if (e.MaxCountdown - span.TotalSeconds > 0)
                    {
                        windowType = "window";
                        span = new TimeSpan(0, 0, (int)e.MaxCountdown) - span;
                    }
                    else
                    {
                        m_Countdown = false;
                        windowType = "behind";
                        span -= new TimeSpan(0, 0, (int)Math.Max(e.MinCountdown, e.MaxCountdown));
                    }
                }
            }

            m_EventTime = span;

            Visibility = Visibility.Visible;

            if (e.StageTypeEnum == MetaEventStage.StageType.Boss)
                Background = BOSS_GRADIENT;
            else if (e.StageTypeEnum == MetaEventStage.StageType.PreEvent)
                Background = PRE_GRADIENT;
            else
            {
                Background = null;

                if (windowType == "window")
                    EventTime.Foreground = WINDOW_FOREGROUND;
                else if (windowType == "behind")
                    EventTime.Foreground = BEHIND_FOREGROUND;
                else
                {
                    EventTime.Foreground = null;
                    Visibility = Visibility.Collapsed;
                }
            }

            if (e.StageId < 0)
            {
                TimeSpan window = new TimeSpan(0, 0, (int)(e.MaxCountdown - e.MinCountdown));

                if (windowType == "behind")
                    EventInfo.Text = "Time Event has been Outdated";
                else if (windowType == "window")
                    EventInfo.Text = string.Format("{0}{1}Spawning Window",
                            (window.Hours > 0 ? string.Format("{0} Hour ", window.Hours) : string.Empty),
                            (window.Minutes > 0 ? string.Format("{0} Minute ", window.Minutes) : string.Empty));
                else
                    EventInfo.Text = string.Empty;
            }
            else
                EventInfo.Text = e.StageName;
        }

        public void Tick()
        {
            m_EventTime += new TimeSpan(0, 0, (m_Countdown ? -1 : 1));

            string format;
            if (m_EventTime.Days > 0)
                format = @"d\.hh\:mm\:ss";
            else if (m_EventTime.Hours > 0)
                format = @"h\:mm\:ss";
            else if (m_EventTime.Minutes > 0)
                format = @"m\:ss";
            else
                format = @"%s";

            EventTime.Text = m_EventTime.ToString(format);
        }
    }
}
