using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GuildWars2.PvPCasterToolbox
{
    internal class ScoreBarAnimator
    {
        private static readonly EasingFunctionBase EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
        private static readonly TimeSpan BoostDuration = TimeSpan.FromSeconds(0.25);
        private static readonly TimeSpan BoostStopDelayDuration = TimeSpan.FromSeconds(0.1);
        private static readonly TimeSpan ScoreStartDelayDuration = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan ScoreDuration = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan ScoreStopDelayDuration = TimeSpan.FromSeconds(0.25);

        private string boostBarTransparentStopName;
        private string boostBarBlackStopName;
        private string scoreBarTransparentStopName;
        private string scoreBarBlackStopName;

        private double currentValue = 0;

        public ScoreBarAnimator(string boostBarTransparentStopName,
                                string boostBarBlackStopName,
                                string scoreBarTransparentStopName,
                                string scoreBarBlackStopName)
        {
            this.boostBarTransparentStopName = boostBarTransparentStopName;
            this.boostBarBlackStopName = boostBarBlackStopName;
            this.scoreBarTransparentStopName = scoreBarTransparentStopName;
            this.scoreBarBlackStopName = scoreBarBlackStopName;
        }

        public void BeginAnimateScore(double newValue, FrameworkElement parent)
        {
            if (newValue != this.currentValue)
            {
                lock (this)
                {
                    if (newValue != this.currentValue)
                    {
                        var boostTransparentAnimation = new DoubleAnimation(newValue, BoostDuration) { EasingFunction = EasingFunction };
                        Storyboard.SetTargetName(boostTransparentAnimation, this.boostBarTransparentStopName);
                        Storyboard.SetTargetProperty(boostTransparentAnimation, new PropertyPath(GradientStop.OffsetProperty));

                        var boostStopAnimation = new DoubleAnimation(newValue, BoostDuration) { EasingFunction = EasingFunction };
                        Storyboard.SetTargetName(boostStopAnimation, this.boostBarBlackStopName);
                        Storyboard.SetTargetProperty(boostStopAnimation, new PropertyPath(GradientStop.OffsetProperty));

                        var scoreTransparentAnimation = new DoubleAnimation(newValue, ScoreDuration) { EasingFunction = EasingFunction };
                        Storyboard.SetTargetName(scoreTransparentAnimation, this.scoreBarTransparentStopName);
                        Storyboard.SetTargetProperty(scoreTransparentAnimation, new PropertyPath(GradientStop.OffsetProperty));

                        var scoreStopAnimation = new DoubleAnimation(newValue, ScoreDuration) { EasingFunction = EasingFunction };
                        Storyboard.SetTargetName(scoreStopAnimation, this.scoreBarBlackStopName);
                        Storyboard.SetTargetProperty(scoreStopAnimation, new PropertyPath(GradientStop.OffsetProperty));

                        if (newValue > this.currentValue)
                        {
                            boostStopAnimation.BeginTime = BoostStopDelayDuration;
                            scoreTransparentAnimation.BeginTime = ScoreStartDelayDuration;
                            scoreStopAnimation.BeginTime = ScoreStartDelayDuration + ScoreStopDelayDuration;
                        }
                        else
                        {
                            scoreTransparentAnimation.BeginTime = BoostStopDelayDuration;
                            boostStopAnimation.BeginTime = ScoreStartDelayDuration;
                            boostTransparentAnimation.BeginTime = ScoreStartDelayDuration + ScoreStopDelayDuration;
                        }

                        var storyboard = new Storyboard();
                        storyboard.Children.Add(boostTransparentAnimation);
                        storyboard.Children.Add(boostStopAnimation);
                        storyboard.Children.Add(scoreTransparentAnimation);
                        storyboard.Children.Add(scoreStopAnimation);
                        storyboard.Begin(parent);

                        this.currentValue = newValue;
                    }
                }
            }
        }
    }
}
