using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace GuildWars2.PvPOcr
{
    public class Gw2Process
    {
        private Process process;

        public Gw2Process()
            => this.process = Process.GetProcesses()
                                     .Where(proc => proc.ProcessName.StartsWith("gw2", StringComparison.CurrentCultureIgnoreCase))
                                     .FirstOrDefault();

        public Bitmap GetBitmap()
        {
            Rectangle rect = this.GetWindowRectangle();

            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBmp = gfxBmp.GetHdc();

            PrintWindow(this.process.MainWindowHandle, hdcBmp, 0);

            gfxBmp.ReleaseHdc(hdcBmp);
            gfxBmp.Dispose();

            return bmp;
        }

        public Rectangle GetWindowRectangle()
        {
            GetWindowRect(this.process.MainWindowHandle, out Rect rect);
            return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect rect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int flags);
    }
}
