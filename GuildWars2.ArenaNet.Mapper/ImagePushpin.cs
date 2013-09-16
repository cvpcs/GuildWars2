using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#if SILVERLIGHT
using Microsoft.Maps.MapControl;
#else
using Microsoft.Maps.MapControl.WPF;
#endif

namespace GuildWars2.ArenaNet.Mapper
{
    public abstract class ImagePushpin : TemplatedPushpin
    {
        private static string POPUP_NAME = "ImagePushpinTemplatePopup";
        private static string POPUPCONTENT_NAME = "ImagePushpinTemplatePopupContent";
        private static string POPUPPATH_NAME = "ImagePushpinTemplatePopupPath";
        private static string IMAGE_NAME = "ImagePushpinTemplateImage";

        private bool m_TemplateApplied = false;
        private object m_SavedToolTip;
        private MapLayer m_SavedMapLayer;
        private int m_SavedMapLayerPosition;

        private BitmapImage m_Image;
        public BitmapImage Image
        {
            get { return m_Image; }
            set { SetImage(value); }
        }
        
        private double m_ImageWidth = 20;
        public double ImageWidth
        {
            get { return m_ImageWidth; }
            set { SetImageSize(value, m_ImageHeight); }
        }

        private double m_ImageHeight = 20;
        public double ImageHeight
        {
            get { return m_ImageHeight; }
            set { SetImageSize(m_ImageWidth, value); }
        }

        private double m_ImageRotation = 0;
        public double ImageRotation
        {
            get { return m_ImageRotation; }
            set { SetImageRotation(value); }
        }

        private object m_PopupContent;
        public object PopupContent
        {
            get { return m_PopupContent; }
            set { SetPopupContent(value); }
        }

#if SILVERLIGHT
        public object ToolTip
        {
            get { return ToolTipService.GetToolTip(this); }
            set { ToolTipService.SetToolTip(this, value); }
        }
#endif

        public ImagePushpin()
            : base("/ImagePushpinTemplate.xaml")
        {
            // make sure the global pushpin's width/height is Auto
            Height = double.NaN;
            Width = double.NaN;

            MouseLeftButtonDown += PushpinClickHandler;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_TemplateApplied = true;

            SetImage(m_Image);
            SetImageSize(m_ImageWidth, m_ImageHeight);
            SetImageRotation(m_ImageRotation);

            if (m_PopupContent != null)
            {
                Rectangle image = (Rectangle)GetTemplateChild(IMAGE_NAME);
                image.MouseLeftButtonDown += PopupClickHandler;
            }
        }

        private void SetImage(BitmapImage img)
        {
            m_Image = img;

            if (m_TemplateApplied)
            {
                Rectangle image = (Rectangle)GetTemplateChild(IMAGE_NAME);
                ((ImageBrush)image.Fill).ImageSource = m_Image;
            }
        }

        private void SetImageSize(double width, double height)
        {
            m_ImageWidth = width;
            m_ImageHeight = height;

            if (m_TemplateApplied)
            {
                Rectangle image = (Rectangle)GetTemplateChild(IMAGE_NAME);
                image.Width = m_ImageWidth;
                image.Height = m_ImageHeight;
                RotateTransform rotate = (RotateTransform)image.RenderTransform;
                rotate.CenterX = m_ImageWidth / 2;
                rotate.CenterY = m_ImageHeight / 2;

                Path path = (Path)GetTemplateChild(POPUPPATH_NAME);
                path.Width = m_ImageWidth;
            }

            if (m_PopupContent != null)
                Margin = new Thickness(-(m_ImageWidth / 2), 0, 0, -(m_ImageHeight / 2));
        }

        private void SetImageRotation(double angle)
        {
            m_ImageRotation = angle;

            if (m_TemplateApplied)
            {
                Rectangle image = (Rectangle)GetTemplateChild(IMAGE_NAME);
                RotateTransform rotate = (RotateTransform)image.RenderTransform;
                rotate.Angle = m_ImageRotation;
                rotate.CenterX = m_ImageWidth / 2;
                rotate.CenterY = m_ImageHeight / 2;
            }
        }

        private void SetPopupContent(object content)
        {
            if (m_PopupContent == null && content != null)
            {
                PositionOrigin = PositionOrigin.BottomLeft;
                Margin = new Thickness(-(m_ImageWidth / 2), 0, 0, -(m_ImageHeight / 2));

                if (m_TemplateApplied)
                {
                    Rectangle image = (Rectangle)GetTemplateChild(IMAGE_NAME);
                    image.MouseLeftButtonDown += PopupClickHandler;
                }
            }
            else if (m_PopupContent != null && content == null)
            {
                PositionOrigin = PositionOrigin.Center;
                Margin = new Thickness(0);

                if (m_TemplateApplied)
                {
                    Rectangle image = (Rectangle)GetTemplateChild(IMAGE_NAME);
                    image.MouseLeftButtonDown -= PopupClickHandler;

                    Grid popup = (Grid)GetTemplateChild(POPUP_NAME);
                    popup.Visibility = Visibility.Collapsed;
                }
            }

            m_PopupContent = content;

            if (m_TemplateApplied)
            {
                ContentPresenter popupContent = (ContentPresenter)GetTemplateChild(POPUPCONTENT_NAME);
                popupContent.Content = m_PopupContent;
            }
        }

        private void PopupClickHandler(object sender, MouseButtonEventArgs e)
        {
            Grid popup = (Grid)GetTemplateChild(POPUP_NAME);

            if (popup.Visibility == Visibility.Collapsed)
            {
                m_SavedToolTip = ToolTip;
                ToolTip = null;
                popup.Visibility = Visibility.Visible;
            }
            else
            {
                popup.Visibility = Visibility.Collapsed;
                ToolTip = m_SavedToolTip;
            }
        }

        private void PushpinClickHandler(object sender, MouseButtonEventArgs e)
        {
            Grid popup = (Grid)GetTemplateChild(POPUP_NAME);
            Map map = (Map)VisualTreeUtility.FindParent(this, typeof(Map));
            map.Children.Remove(this);

            if (popup.Visibility == Visibility.Visible)
            {
                if (m_SavedMapLayer == null)
                {
                    m_SavedMapLayer = (MapLayer)VisualTreeUtility.FindParent(this, typeof(MapLayer));

                    for (int i = 0; i < m_SavedMapLayer.Children.Count; i++)
                    {
                        if (m_SavedMapLayer.Children[i] == this)
                        {
                            m_SavedMapLayerPosition = i;
                            break;
                        }
                    }

                    m_SavedMapLayer.Children.Remove(this);
                }

                map.Children.Add(this);
            }
            else if (m_SavedMapLayer != null)
            {
                m_SavedMapLayer.Children.Insert(m_SavedMapLayerPosition, this);
                m_SavedMapLayer = null;
                m_SavedMapLayerPosition = -1;
            }
        }
    }
}