using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPCasterToolbox
{
    public class Gw2ScreenshotPublisher : AsyncDataPublisherBase<Bitmap>
    {
        private Process process;

        public Gw2ScreenshotPublisher(ILogger<Gw2ScreenshotPublisher> logger)
            : base(logger)
        { }

        protected override Task<Bitmap> GetDataAsync(CancellationToken token)
        {
            if (!IsAvailable && !TryFindGw2Process(out this.process))
            {
                throw new InvalidOperationException("Gw2 process not found");
            }

            try
            {
                GetWindowRect(this.process.MainWindowHandle, out Rect rect);
                var bitmap = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, PixelFormat.Format32bppArgb);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr gfxHdc = graphics.GetHdc();
                    PrintWindow(this.process.MainWindowHandle, gfxHdc, PrintWindowFlags.ClientOnly);
                    graphics.ReleaseHdc(gfxHdc);
                }

                return Task.FromResult(bitmap);
            }
            catch
            {
                this.process = null;
                throw;
            }
        }

        private bool IsAvailable
            => this.process?.HasExited == false;

        private static bool TryFindGw2Process(out Process process)
        {
            process = Process.GetProcesses()
                             .Where(proc => proc.ProcessName.StartsWith("gw2", StringComparison.CurrentCultureIgnoreCase))
                             .FirstOrDefault();
            return process != null;
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
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, PrintWindowFlags flags);

        [Flags]
        private enum PrintWindowFlags
        {
            None = 0,
            ClientOnly = 0x1
        }
    }
}
