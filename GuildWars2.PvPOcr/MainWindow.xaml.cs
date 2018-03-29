using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GuildWars2.Overlay.Controls;
using ImageMagick;
using Tesseract;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class MainWindow : ClickThroughTransparentWindow
    {
        private readonly ScoreBarAnimator RedScoreBarAnimator;
        private readonly ScoreBarAnimator BlueScoreBarAnimator;
        private readonly OcrManager ocrManager;

        public MainWindow()
        {
            InitializeComponent();

            this.RedScoreBarAnimator = new ScoreBarAnimator(nameof(this.RedBoostBar_TransparentStop),
                                                            nameof(this.RedBoostBar_BlackStop),
                                                            nameof(this.RedScoreBar_TransparentStop),
                                                            nameof(this.RedScoreBar_BlackStop));
            this.BlueScoreBarAnimator = new ScoreBarAnimator(nameof(this.BlueBoostBar_TransparentStop),
                                                             nameof(this.BlueBoostBar_BlackStop),
                                                             nameof(this.BlueScoreBar_TransparentStop),
                                                             nameof(this.BlueScoreBar_BlackStop));
            this.ocrManager = new OcrManager();
            this.ocrManager.ScoresRead += (scores) => Dispatcher.Invoke(() => 
            {
                this.RedScoreBarAnimator.BeginAnimateScore(scores.RedPercentage, this);
                this.BlueScoreBarAnimator.BeginAnimateScore(scores.BluePercentage, this);
            });
            this.ocrManager.ScoresReset += () => Dispatcher.Invoke(() =>
            {
                this.RedScoreBarAnimator.BeginAnimateScore(0, this);
                this.BlueScoreBarAnimator.BeginAnimateScore(0, this);
            });

            this.ocrManager.StartThread();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                WindowUI.Visibility = (WindowUI.IsVisible ? Visibility.Hidden : Visibility.Visible);
            }

            if (e.Key == Key.LeftCtrl)
            {
                this.IsClickThroughTransparent = !this.IsClickThroughTransparent;
            }

            base.OnKeyDown(e);
        }
    }
}
