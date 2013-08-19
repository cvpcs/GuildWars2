using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    public abstract class ImagePushpin : Pushpin
    {
        private static string IMAGEBRUSH_NAME = "ImagePushpinTemplateBrush";
        private static ControlTemplate TEMPLATE = (ControlTemplate)XamlReader.Parse(string.Format(@"
                <ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                 xmlns:l=""clr-namespace:GuildWars2.ArenaNet.Mapper;assembly=GuildWars2.ArenaNet.Mapper""
                                 xmlns:m=""clr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPF"">
                    <Rectangle Width=""{{Binding Path=Width, RelativeSource={{RelativeSource FindAncestor,AncestorType=m:Pushpin}}}}""
                               Height=""{{Binding Path=Height, RelativeSource={{RelativeSource FindAncestor,AncestorType=m:Pushpin}}}}"">
                        <Rectangle.Fill>
                            <ImageBrush x:Name=""{0}""
                                        ImageSource=""{{Binding Path=Image, RelativeSource={{RelativeSource FindAncestor,AncestorType=l:ImagePushpin}}}}"" />
                        </Rectangle.Fill>
                    </Rectangle>
                </ControlTemplate>
                ", IMAGEBRUSH_NAME));

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