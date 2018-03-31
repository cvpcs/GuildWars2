using System.Drawing;
using System.Windows;

namespace GuildWars2.PvPOcr
{
    public static class WindowExtensions
    {
        public static RectangleF GetWindowRect(this Window window)
            => new RectangleF
            {
                X = (float)window.Left,
                Y = (float)window.Top,
                Width = (float)window.Width,
                Height = (float)window.Height
            };

        public static void SetWindowRect(this Window window, RectangleF rect)
        {
            window.Left = rect.Left;
            window.Top = rect.Top;
            window.Width = rect.Width;
            window.Height = rect.Height;
        }
    }
}
