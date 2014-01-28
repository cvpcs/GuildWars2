using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GuildWars2.ArenaNet.Mapper
{
    public static class ResourceUtility
    {
        public static BitmapImage LoadBitmapImage(string resourceUri)
        {
            return new BitmapImage(new Uri(string.Format("pack://application:,,,/GuildWars2.ArenaNet.Mapper;component{0}", resourceUri)));
        }

        public static ControlTemplate LoadControlTemplate(string resourceUri)
        {
            ControlTemplate template;

            template = (ControlTemplate)Application.LoadComponent(
                new Uri(string.Format("/GuildWars2.ArenaNet.Mapper;component{0}", resourceUri), UriKind.Relative));

            return template;
        }
    }
}
