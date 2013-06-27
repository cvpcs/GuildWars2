using System;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class TaskPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE;

        static TaskPushpin()
        {
            IMAGE = new BitmapImage();
            IMAGE.BeginInit();
            IMAGE.StreamSource = Application.GetResourceStream(new Uri("/Resources/renown_heart.png", UriKind.Relative)).Stream;
            IMAGE.EndInit();
        }

        public TaskPushpin(Task task)
            : base()
        {
            Width = 20;
            Height = 20;

            PositionOrigin = PositionOrigin.Center;

            Image = IMAGE;

            if (!string.IsNullOrWhiteSpace(task.Objective))
                ToolTip = string.Format("{0} ({1})", task.Objective, task.Level);
        }
    }
}
