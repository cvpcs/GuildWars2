using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageMagick;

namespace GuildWars2.PvPOcr
{
    public static class WriteableBitmapExtensions
    {
        public static void WriteMagickImage(this WriteableBitmap writeableBitmap, IMagickImage magickImage)
        {
            using (Bitmap bitmap = magickImage.ToBitmap())
            {
                writeableBitmap.WriteBitmap(bitmap);
            }
        }

        public static void WriteBitmap(this WriteableBitmap writableBitmap, Bitmap bitmap, Rectangle? region = null)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            writableBitmap.WritePixels(region == null ? new Int32Rect(0, 0, bitmap.Width, bitmap.Height)
                                                      : new Int32Rect(region.Value.X, region.Value.Y, region.Value.Width, region.Value.Height),
                                       bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
        }
    }
}
