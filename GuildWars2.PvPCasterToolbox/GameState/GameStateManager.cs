using System;
using System.Collections.Generic;
using System.Drawing;
using GuildWars2.PvPCasterToolbox.Configuration;
using GW2NET.MumbleLink;
using ImageMagick;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace GuildWars2.PvPCasterToolbox.GameState
{
    // TODO: move to its own subnamespace with its interfaces and split into more subclasses?
    // TODO: change this to not inherit game state but instead just pass it as part of the scores read action
    public class GameStateManager : IGameState
    {
        private const int HOTM_MAP_ID = 350;

        // TODO: do i like these events? is there a better way to handle the screenshot processing?
        // TODO: use actual event handlers and event args
        public event Action<IGameState> ScoresRead;
        public event Action<Bitmap, IEnumerable<(Rectangle rect, Bitmap bitmap)>> ProcessedScreenshotSections;

        private TesseractEngine tesseractEngine;
        private AppConfig appConfig;
        private ILogger logger;

        private TeamState red = new TeamState();
        public ITeamState Red => red;

        private TeamState blue = new TeamState();
        public ITeamState Blue => blue;

        private int? currentMapId = -1;

        public GameStateManager(Gw2MumbleLinkPublisher gw2MumbleLinkPublisher,
                                Gw2ScreenshotPublisher gw2ScreenshotPublisher,
                                TesseractEngine tesseractEngine,
                                AppConfig appConfig,
                                ILogger<GameStateManager> logger)
        {
            this.tesseractEngine = tesseractEngine;
            this.appConfig = appConfig;
            this.logger = logger;

            gw2MumbleLinkPublisher.DataAvailable += ProcessMumbleLinkData;
            gw2ScreenshotPublisher.DataAvailable += ProcessScreenshot;
        }

        private void ProcessScreenshot(Bitmap screenshot)
        {
            try
            {
                this.logger.LogDebug("Processing screenshot to determine scores...");
                using (var screenshotImage = new MagickImage(screenshot))
                using (IMagickImage modifiedScreenshotImage = screenshotImage.Clone())
                {
                    // mess with the image to make it easier to OCR
                    modifiedScreenshotImage.Contrast();
                    modifiedScreenshotImage.Grayscale(PixelIntensityMethod.Rec709Luminance);
                    modifiedScreenshotImage.LevelColors(MagickColor.FromRgb(128, 128, 128), MagickColor.FromRgb(255, 255, 255));
                    modifiedScreenshotImage.Negate(Channels.RGB);

                    using (var modifiedScreenshot = modifiedScreenshotImage.ToBitmap())
                    {
                        var success = true;
                        using (Page section = this.tesseractEngine.Process(modifiedScreenshot, GetTesseractRect(this.appConfig.RedSection), PageSegMode.SingleWord))
                        {
                            string redText = section.GetText();
                            this.logger.LogTrace($"Red section OCR result: {redText}");

                            bool redSuccess = this.red.TryProcessScore(redText);
                            this.logger.LogTrace($"Red section score processing result: {(redSuccess ? "success" : "failure")} - {this.red.ToString()}");
                            success = success && redSuccess;
                        }
                        using (Page section = this.tesseractEngine.Process(modifiedScreenshot, GetTesseractRect(this.appConfig.BlueSection), PageSegMode.SingleWord))
                        {
                            string blueText = section.GetText();
                            this.logger.LogTrace($"Blue section OCR result: {blueText}");

                            bool blueSuccess = this.blue.TryProcessScore(blueText);
                            this.logger.LogTrace($"Blue section score processing result: {(blueSuccess ? "success" : "failure")} - {this.blue.ToString()}");
                            success = success && blueSuccess;
                        }

                        if (success)
                        {
                            this.logger.LogInformation($"Game state: [red={this.red.Score} ({this.red.Kills} kills); blue={this.blue.Score} ({this.blue.Kills} kills)]");

                            if (this.red.Score == 0 && this.blue.Score == 0)
                            {
                                this.Reset();
                            }

                            this.ScoresRead?.Invoke(this);
                        }
                        else
                        {
                            this.logger.LogWarning("Unable to read game state");
                        }

                        if (this.ProcessedScreenshotSections != null)
                        {
                            // only perform this image processing if we have an event handler for processed screenshots
                            using (IMagickImage modifiedScreenshotRedSectionImage = modifiedScreenshotImage.Clone(new MagickGeometry(this.appConfig.RedSection)))
                            using (IMagickImage modifiedScreenshotBlueSectionImage = modifiedScreenshotImage.Clone(new MagickGeometry(this.appConfig.BlueSection)))
                            using (Bitmap modifiedScreenshotRedSection = modifiedScreenshotRedSectionImage.ToBitmap())
                            using (Bitmap modifiedScreenshotBlueSection = modifiedScreenshotBlueSectionImage.ToBitmap())
                            {
                                this.ProcessedScreenshotSections(screenshot, new List<(Rectangle, Bitmap)>
                                {
                                    (this.appConfig.RedSection, modifiedScreenshotRedSection),
                                    (this.appConfig.BlueSection, modifiedScreenshotBlueSection)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, string.Empty);
            }
        }

        private void ProcessMumbleLinkData(Avatar avatar)
        {
            if (this.currentMapId != avatar.Context.MapId)
            {
                this.currentMapId = avatar.Context.MapId;
                if (this.currentMapId != HOTM_MAP_ID)
                {
                    this.Reset();
                    this.ScoresRead?.Invoke(this);
                }
            }
        }

        private void Reset()
        {
            this.logger.LogDebug("Resetting scores");
            this.red.Reset();
            this.blue.Reset();
        }

        private static Tesseract.Rect GetTesseractRect(Rectangle rect)
            => new Rect(rect.X, rect.Y, rect.Width, rect.Height);

        private class TeamState : ITeamState
        {
            public int Score { get; private set; } = -1;
            public double ScorePercentage => Score / 500.0;

            public int Kills { get; private set; } = 0;

            public int ScoreDelta { get; private set; } = 0;
            public int KillsDelta { get; private set; } = 0;

            private int consecutiveInvalidDeltaFailures = 0;

            public bool TryProcessScore(string newScoreText)
            {
                if (int.TryParse(newScoreText.Trim()
                                          .ToLower()
                                          .Replace('o', '0'),
                                 out int newScore))
                {
                    // if the score is < 0, we haven't read a score yet so assume delta of 0 and just accept the new score
                    int scoreDelta = Score >= 0 ? newScore - Score : 0;

                    // ignore faulty read where +5 for kill could be read as part of the score:
                    //   - delta > 50 when last score < 10 (e.g. +5 9 -> 59)
                    //   - delta > 178 since maximum tick score should be 178 (+150 lord, +25 5x kill, +3 3x node)
                    // this is overridden and the score is considered valid if we fail here 10 times in a row in order
                    // to catch cases where we have swapped games and the deltas are unreliable
                    if (((Score < 10 && scoreDelta > 50) ||
                         scoreDelta > 178) &&
                        this.consecutiveInvalidDeltaFailures++ < 10)
                    {
                        return false;
                    }

                    this.consecutiveInvalidDeltaFailures = 0;

                    ScoreDelta = scoreDelta;
                    Score = newScore;

                    // if our score delta is between 0 and 15, calculate possible kill-related increases
                    KillsDelta = (scoreDelta > 0 && scoreDelta < 15) ? scoreDelta / 5 : 0;
                    Kills += KillsDelta;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                ScoreDelta = 0;
                Score = 0;

                KillsDelta = 0;
                Kills = 0;
            }

            public override string ToString()
                => $"[score={Score}; scoreDelta={ScoreDelta}; kills={Kills}; killsDelta={KillsDelta}]";
        }
    }
}
