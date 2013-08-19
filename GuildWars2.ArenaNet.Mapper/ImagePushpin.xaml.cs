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

using Microsoft.Maps.MapControl.WPF;

namespace GuildWars2.ArenaNet.Mapper
{
    /// <summary>
    /// Interaction logic for ImagePushpin.xaml
    /// </summary>
    public partial class ImagePushpin : Pushpin
    {
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
                    ImageBrush brush = (ImageBrush)GetTemplateChild("ImagePushpinTemplateBrush");
                    brush.ImageSource = m_Image;
                }
            }
        }

        public ImagePushpin()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_TemplateApplied = true;
        }
    }
}
