using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GuildWars2.Overlay.Controls;
using WindowsPoint = System.Windows.Point;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for ScoreBarWindow.xaml
    /// </summary>
    public partial class ScoreBarWindow : ClickThroughTransparentWindow
    {
        private static readonly WindowsPoint GradientOpacityLeftPoint = new WindowsPoint(0, 0);
        private static readonly WindowsPoint GradientOpacityRightPoint = new WindowsPoint(1, 0);

        private readonly ScoreBarAnimator scoreBarAnimator;

        public ScoreBarWindow(Uri backgroundBarImageUri, Uri boostBarImageUri, Uri scoreBarImageUri, bool isFlipped = false, bool overlayMode = true, RectangleF? position = null)
        {
            InitializeComponent();

            this.BoostBar_GradientOpacityMask.StartPoint =
            this.ScoreBar_GradientOpacityMask.StartPoint = isFlipped ? GradientOpacityRightPoint : GradientOpacityLeftPoint;
            this.BoostBar_GradientOpacityMask.EndPoint =
            this.ScoreBar_GradientOpacityMask.EndPoint = isFlipped ? GradientOpacityLeftPoint : GradientOpacityRightPoint;

            this.BackgroundBar.Source = new BitmapImage(backgroundBarImageUri);
            this.BoostBar.Source = new BitmapImage(boostBarImageUri);
            this.ScoreBar.Source = new BitmapImage(scoreBarImageUri);

            this.scoreBarAnimator = new ScoreBarAnimator(nameof(this.BoostBar_GradientOpacityMask_TransparentStop),
                                                         nameof(this.BoostBar_GradientOpacityMask_BlackStop),
                                                         nameof(this.ScoreBar_GradientOpacityMask_TransparentStop),
                                                         nameof(this.ScoreBar_GradientOpacityMask_BlackStop));

            this.AllowsTransparency = overlayMode;
            this.GreenScreen.Visibility = overlayMode ? Visibility.Hidden : Visibility.Visible;

            if (position != null)
            {
                this.SetWindowRect(position.Value);
            }
        }

        public void SetScoreBarFill(double percentage)
            => this.scoreBarAnimator.BeginAnimateScore(percentage, this);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                this.WindowUI.Visibility = (this.WindowUI.IsVisible ? Visibility.Hidden : Visibility.Visible);
            }

            if (e.Key == Key.LeftCtrl)
            {
                this.IsClickThroughTransparent = !this.IsClickThroughTransparent;
            }

            base.OnKeyDown(e);
        }
    }
}
