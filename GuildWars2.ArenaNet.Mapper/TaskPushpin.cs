using System;
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
    public class TaskPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = new BitmapImage(new Uri("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component/Resources/renown_heart.png"));

        public TaskPushpin(Task task)
            : base()
        {
            Image = IMAGE;

            if (!string.IsNullOrWhiteSpace(task.Objective))
                ToolTip = string.Format("{0} ({1})", task.Objective, task.Level);
        }
    }
}
