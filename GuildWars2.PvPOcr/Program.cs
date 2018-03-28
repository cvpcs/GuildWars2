using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ImageMagick;
using Tesseract;

namespace GuildWars2.PvPOcr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var gw2Process = new Gw2Process();

            OverlayHandler.Initialize();

            var ocrOptions = new Dictionary<string, object>
            {
                ["load_system_dawg"] = false,
                ["load_freq_dawg"] = false
            };

            using (var ocrEngine = new TesseractEngine("./tessdata", "eng", EngineMode.Default, null, ocrOptions, false))
            {
                while (true)
                {
                    using (Bitmap screenshot = gw2Process.GetBitmap())
                    using (var screenshotImage = new MagickImage(screenshot))
                    {
                        // mess with the image to make it easier to OCR
                        screenshotImage.Contrast();
                        screenshotImage.Grayscale(PixelIntensityMethod.Rec709Luminance);
                        screenshotImage.LevelColors(MagickColor.FromRgb(128, 128, 128), MagickColor.FromRgb(255, 255, 255));
                        screenshotImage.Negate(Channels.RGB);

                        using (Bitmap modifiedScreenshot = screenshotImage.ToBitmap())
                        {
                            int redScore, blueScore = -1;

                            using (Page redSection = ocrEngine.Process(modifiedScreenshot, new Rect(820, 0, 80, 40), PageSegMode.SingleWord))
                            {
                                redScore = ProcessScore(redSection);
                            }
                            using (Page blueSection = ocrEngine.Process(modifiedScreenshot, new Rect(1020, 0, 80, 40), PageSegMode.SingleWord))
                            {
                                blueScore = ProcessScore(blueSection);
                            }

                            Console.WriteLine($"Red: {redScore}, Blue: {blueScore}");

                            OverlayHandler.LoadScores(redScore, blueScore);
                        }
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private static int ProcessScore(Page page)
        {
            string data = page.GetText()
                              .Trim()
                              .ToLower()
                              .Replace('o', '0');
            
            if (int.TryParse(data, out int result))
            {
                return result;
            }

            return -1;
        }
    }
}
