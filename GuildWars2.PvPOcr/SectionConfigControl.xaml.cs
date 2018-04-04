using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace GuildWars2.PvPOcr
{
    /// <summary>
    /// Interaction logic for WindowUI.xaml
    /// </summary>
    public partial class SectionConfigControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
                                                                                              typeof(string),
                                                                                              typeof(SectionConfigControl));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private int maxSectionWidth = 1920;
        public int MaxSectionWidth
        {
            get => this.maxSectionWidth;
            set { if (this.maxSectionWidth != value) { this.maxSectionWidth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxSectionWidth))); } }
        }

        private int maxSectionHeight = 1080;
        public int MaxSectionHeight
        {
            get => this.maxSectionHeight;
            set { if (this.maxSectionHeight != value) { this.maxSectionHeight = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxSectionHeight))); } }
        }

        private int sectionX = 0;
        public int SectionX
        {
            get => this.sectionX;
            set { if (this.sectionX != value) { this.sectionX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionX))); } }
        }

        private int sectionY = 0;
        public int SectionY
        {
            get => this.sectionY;
            set { if (this.sectionY != value) { this.sectionY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionY))); } }
        }

        private int sectionWidth = 0;
        public int SectionWidth
        {
            get => this.sectionWidth;
            set { if (this.sectionWidth != value) { this.sectionWidth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionWidth))); } }
        }

        private int sectionHeight = 0;
        public int SectionHeight
        {
            get => this.sectionHeight;
            set { if (this.sectionHeight != value) { this.sectionHeight = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionHeight))); } }
        }

        private int scoreBarHue = 0;
        public int ScoreBarHue
        {
            get => this.scoreBarHue;
            set { if (this.scoreBarHue != value) { this.scoreBarHue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarHue))); } }
        }

        private int scoreBarSaturation = 0;
        public int ScoreBarSaturation
        {
            get => this.scoreBarSaturation;
            set { if (this.scoreBarSaturation != value) { this.scoreBarSaturation = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarSaturation))); } }
        }

        private int scoreBarBrightness = 0;
        public int ScoreBarBrightness
        {
            get => this.scoreBarBrightness;
            set { if (this.scoreBarBrightness != value) { this.scoreBarBrightness = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarBrightness))); } }
        }

        public Rectangle SectionRect
        {
            get => new Rectangle(SectionX, SectionY, SectionWidth, SectionHeight);
            set
            {
                SectionX = value.X;
                SectionY = value.Y;
                SectionWidth = value.Width;
                SectionHeight = value.Height;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionRect)));
            }
        }

        public ImageModulationParameters ScoreBarModulationParameters
        {
            get => new ImageModulationParameters(ScoreBarHue, ScoreBarSaturation, ScoreBarBrightness);
            set
            {
                ScoreBarHue = value.Hue;
                ScoreBarSaturation = value.Saturation;
                ScoreBarBrightness = value.Brightness;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreBarModulationParameters)));
            }
        }

        public SectionConfigControl()
        {
            InitializeComponent();
        }
    }
}
