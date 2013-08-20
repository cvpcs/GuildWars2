using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public abstract class ImagePushpin : Pushpin
    {
        private static string IMAGEBRUSH_NAME = "ImagePushpinTemplateBrush";
        private static ControlTemplate TEMPLATE = (ControlTemplate)Application.LoadComponent(
                new Uri("/ImagePushpinTemplate.xaml", UriKind.Relative));

        private bool m_TemplateApplied = false;

        private BitmapImage m_Image;
        public BitmapImage Image
        {
            get { return m_Image; }
            set
            {
                m_Image = value;

                if (m_TemplateApplied)
                {
                    ImageBrush brush = (ImageBrush)GetTemplateChild(IMAGEBRUSH_NAME);
                    brush.ImageSource = m_Image;
                }
            }
        }

        public ImagePushpin()
        {
            Width = 20;
            Height = 20;

            PositionOrigin = PositionOrigin.Center;

            Template = TEMPLATE;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_TemplateApplied = true;
        }
    }
}