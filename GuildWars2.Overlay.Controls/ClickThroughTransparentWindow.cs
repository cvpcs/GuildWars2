using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GuildWars2.Overlay.Controls
{
    public class ClickThroughTransparentWindow : Window
    {
        private WindowInteropHelper interop;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.interop = new WindowInteropHelper(this);

            // set window to layered
            int initialStyle = GetWindowLong(this.interop.Handle, GWL_EXSTYLE);
            SetWindowLong(this.interop.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);

        protected bool IsClickThroughTransparent
        {
            get
            {
                int style = GetWindowLong(this.interop.Handle, GWL_EXSTYLE);
                return (style & WS_EX_TRANSPARENT) == WS_EX_TRANSPARENT;
            }
            set
            {
                int style = GetWindowLong(this.interop.Handle, GWL_EXSTYLE);

                if (value)
                {
                    style |= WS_EX_TRANSPARENT;
                }
                else
                {
                    style &= ~WS_EX_TRANSPARENT;
                }

                SetWindowLong(this.interop.Handle, GWL_EXSTYLE, style);
            }
        }
    }
}
