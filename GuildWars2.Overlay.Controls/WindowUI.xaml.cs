using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace GuildWars2.Overlay.Controls
{
    /// <summary>
    /// Interaction logic for WindowUI.xaml
    /// </summary>
    public partial class WindowUI : UserControl
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 2;
        private const int HTBOTTOMRIGHT = 17;

        public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register(nameof(CloseButtonVisibility),
                                                                                                              typeof(Visibility),
                                                                                                              typeof(WindowUI));

        public Visibility CloseButtonVisibility
        {
            get => (Visibility)GetValue(CloseButtonVisibilityProperty);
            set => SetValue(CloseButtonVisibilityProperty, value);
        }

        public WindowUI()
        {
            InitializeComponent();

            Canvas.Cursor = Cursors.SizeAll;
            Canvas.MouseDown += Canvas_MouseDown;

            CloseButton.Cursor = Cursors.Arrow;
            CloseButton.MouseDown += CloseButton_MouseDown;

            ResizeButton.Cursor = Cursors.SizeNWSE;
            ResizeButton.MouseDown += ResizeButton_MouseDown;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr hWnd = ((HwndSource)HwndSource.FromVisual(Window.GetWindow(this))).Handle;
            ReleaseCapture();
            SendMessage(hWnd, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            e.Handled = true;
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).Close();
            e.Handled = true;
        }

        private void ResizeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr hWnd = ((HwndSource)HwndSource.FromVisual(Window.GetWindow(this))).Handle;
            ReleaseCapture();
            SendMessage(hWnd, WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, 0);
            e.Handled = true;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
    }
}
