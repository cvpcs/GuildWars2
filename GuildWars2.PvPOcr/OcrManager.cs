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
        public event Action<Size> CapturedScreenshot;
        public event Action<ProcessedScreenshotEventArgs> ProcessedScreenshot;

        public Rectangle RedSection { get; set; } = new Rectangle(0, 0, 1, 1);
        public Rectangle BlueSection { get; set; } = new Rectangle(0, 0, 1, 1);

        private Task runningTask;
        private CancellationTokenSource runningTaskTokenSource;

        public void StartThread()
        {
            if (this.runningTask != null)
            {
                throw new InvalidOperationException();
            }

            this.runningTaskTokenSource = new CancellationTokenSource();
            CancellationToken token = this.runningTaskTokenSource.Token;
            this.runningTask = Task.Factory.StartNew(async () =>
            {
                var gw2Process = new Gw2Process();

                using (var ocrEngine = new TesseractEngine("./tessdata", "eng", EngineMode.Default, null, TesseractOptions, false))
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        using (Bitmap screenshot = gw2Process.GetBitmap())
                        using (var screenshotImage = new MagickImage(screenshot))
                        using (IMagickImage modifiedScreenshotImage = screenshotImage.Clone())
                        {
                            this.CapturedScreenshot?.Invoke(screenshot.Size);

                            // mess with the image to make it easier to OCR
                            modifiedScreenshotImage.Contrast();
                            modifiedScreenshotImage.Grayscale(PixelIntensityMethod.Rec709Luminance);
                            modifiedScreenshotImage.LevelColors(MagickColor.FromRgb(128, 128, 128), MagickColor.FromRgb(255, 255, 255));
                            modifiedScreenshotImage.Negate(Channels.RGB);

                            using (var modifiedScreenshot = modifiedScreenshotImage.ToBitmap())
                            {
                                string redText, blueText = string.Empty;
                                using (Page section = ocrEngine.Process(modifiedScreenshot, GetTesseractRect(RedSection), PageSegMode.SingleWord))
                                {
                                    redText = section.GetText();
                                }
                                using (Page section = ocrEngine.Process(modifiedScreenshot, GetTesseractRect(BlueSection), PageSegMode.SingleWord))
                                {
                                    blueText = section.GetText();
                                }

                                var scores = new Scores
                                {
                                    Red = ProcessScore(redText),
                                    Blue = ProcessScore(blueText)
                                };

                                this.ScoresRead?.Invoke(scores);

                                if (this.ProcessedScreenshot != null)
                                {
                                    // only perform this image processing if we have an event handler for processed screenshots
                                    using (IMagickImage modifiedScreenshotRedSectionImage = modifiedScreenshotImage.Clone(new MagickGeometry(RedSection)))
                                    using (IMagickImage modifiedScreenshotBlueSectionImage = modifiedScreenshotImage.Clone(new MagickGeometry(BlueSection)))
                                    using (Bitmap modifiedScreenshotRedSection = modifiedScreenshotRedSectionImage.ToBitmap())
                                    using (Bitmap modifiedScreenshotBlueSection = modifiedScreenshotBlueSectionImage.ToBitmap())
                                    {
                                        var processedScreenshotEventArgs = new ProcessedScreenshotEventArgs
                                        {
                                            Screenshot = screenshot,

                                            RedSection = RedSection,
                                            RedSectionPreProcessedScreenshot = modifiedScreenshotRedSection,
                                            RedTextResult = redText,

                                            BlueSection = BlueSection,
                                            BlueSectionPreProcessedScreenshot = modifiedScreenshotBlueSection,
                                            BlueTextResult = blueText,

                                            Result = scores
                                        };

                                        this.ProcessedScreenshot(processedScreenshotEventArgs);
                                    }
                                }
                            }
                        }

                        await Task.Delay(100, token);
                    }
                }
            });
        }

        public void StopThread()
        {
            this.runningTaskTokenSource?.Cancel();
            this.runningTask?.Wait(TimeSpan.FromSeconds(5));
            this.runningTaskTokenSource?.Dispose();
            this.runningTaskTokenSource = null;
            this.runningTask = null;
        }

        private static Tesseract.Rect GetTesseractRect(Rectangle rect)
            => new Rect(rect.X, rect.Y, rect.Width, rect.Height);

        private static int ProcessScore(string scoreText)
        {
            if (int.TryParse(scoreText.Trim()
                                      .ToLower()
                                      .Replace('o', '0'),
                             out int result))
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

        public struct ProcessedScreenshotEventArgs
        {
            public Bitmap Screenshot;

            public Bitmap RedSectionPreProcessedScreenshot;
            public Rectangle RedSection;
            public string RedTextResult;

            public Bitmap BlueSectionPreProcessedScreenshot;
            public Rectangle BlueSection;
            public string BlueTextResult;

            public Scores Result;
        }
    }
}
