using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
using System.IO;
using System.Windows.Markup;

using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public abstract class ImagePushpin : Pushpin
    {
        private static string IMAGEBRUSH_NAME = "ImagePushpinTemplateBrush";
        private static ControlTemplate TEMPLATE;

        static ImagePushpin()
        {
#if SILVERLIGHT
            using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri("/ImagePushpinTemplate.xaml", UriKind.Relative)).Stream))
            {
                TEMPLATE = (ControlTemplate)XamlReader.Load(sr.ReadToEnd());
            }
#else
            TEMPLATE = (ControlTemplate)Application.LoadComponent(
                new Uri("/ImagePushpinTemplate.xaml", UriKind.Relative));
#endif
        }

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

#if SILVERLIGHT
        public string ToolTip
        {
            get { return ToolTipService.GetToolTip(this).ToString(); }
            set { ToolTipService.SetToolTip(this, value); }
        }
#endif

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