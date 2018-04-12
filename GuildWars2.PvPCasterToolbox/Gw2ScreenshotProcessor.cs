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
    public class Gw2ScreenshotProcessor : AsyncProcessorBase
    {
        private static readonly TimeSpan LoopShortDelay = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan LoopLongDelay = TimeSpan.FromSeconds(5);

        public event Action<Bitmap> ScreenshotCaptured;

        private Process process;

        public Gw2ScreenshotProcessor(ILogger<Gw2ScreenshotProcessor> logger)
            : base(logger)
        { }

        protected override async Task ProcessIterationAsync(CancellationToken token)
        {
            if (ScreenshotCaptured != null)
            {
                try
                {
                    using (Bitmap screenshot = this.GetBitmap())
                    {
                        this.ScreenshotCaptured.Invoke(screenshot);
                    }

                    await Task.Delay(LoopShortDelay, token);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, string.Empty);
                    await Task.Delay(LoopLongDelay, token);
                }
            }
            else
            {
                this.logger.LogTrace("No capture handlers configured, ignoring");
                await Task.Delay(LoopLongDelay, token);
            }
        }

        private bool IsAvailable
            => this.process?.HasExited == false;

        private Bitmap GetBitmap()
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

                return bitmap;
            }
            catch
            {
                this.process = null;
                throw;
            }
        }

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
