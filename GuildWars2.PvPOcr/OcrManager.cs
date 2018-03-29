using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Tesseract;

namespace GuildWars2.PvPOcr
{
    public class OcrManager
    {
        private static readonly IDictionary<string, object> TesseractOptions = new Dictionary<string, object>
        {
            ["load_system_dawg"] = false,
            ["load_freq_dawg"] = false
        };

        public event Action<Scores> ScoresRead;
        public event Action ScoresReset;
        private Task runningTask;
        private CancellationTokenSource runningTaskTokenSource;

        public void StartThread()
        {
            if (this.runningTask != null)
            {
                throw new InvalidOperationException();
            }

            this.runningTaskTokenSource = new CancellationTokenSource();
            this.runningTask = Task.Run(() =>
            {
                var gw2Process = new Gw2Process();
                var numBadReads = 0;

                using (var ocrEngine = new TesseractEngine("./tessdata", "eng", EngineMode.Default, null, TesseractOptions, false))
                {
                    while (!this.runningTaskTokenSource.Token.IsCancellationRequested)
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
                                var scores = new Scores();

                                using (Page redSection = ocrEngine.Process(modifiedScreenshot, new Tesseract.Rect(820, 0, 80, 40), PageSegMode.SingleWord))
                                {
                                    scores.Red = ProcessScore(redSection);
                                }
                                using (Page blueSection = ocrEngine.Process(modifiedScreenshot, new Tesseract.Rect(1020, 0, 80, 40), PageSegMode.SingleWord))
                                {
                                    scores.Blue = ProcessScore(blueSection);
                                }

                                if (scores.IsValid)
                                {
                                    this.ScoresRead?.Invoke(scores);
                                    numBadReads = 0;
                                }
                                else if (numBadReads++ > 5)
                                {
                                    this.ScoresReset?.Invoke();
                                }
                            }
                        }

                        Thread.Sleep(100);
                    }
                }

            }, this.runningTaskTokenSource.Token);
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

        public class Scores
        {
            public int Red = -1;
            public int Blue = -1;

            public double RedPercentage => Red / 500.0;
            public double BluePercentage => Blue / 500.0;

            public bool IsValid => Red >= 0 && Red <= 600 && Blue >= 0 && Blue <= 600;
        }
    }
}
