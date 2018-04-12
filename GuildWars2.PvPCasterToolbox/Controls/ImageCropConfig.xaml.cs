using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace GuildWars2.PvPCasterToolbox.Controls
{
    /// <summary>
    /// Interaction logic for WindowUI.xaml
    /// </summary>
    public partial class ImageCropConfig : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int maxSectionWidth = (int)SystemParameters.PrimaryScreenWidth;
        public int MaxSectionWidth
        {
            get => this.maxSectionWidth;
            set { if (this.maxSectionWidth != value) { this.maxSectionWidth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxSectionWidth))); } }
        }

        private int maxSectionHeight = (int)SystemParameters.PrimaryScreenHeight;
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

        private int sectionWidth = 1;
        public int SectionWidth
        {
            get => this.sectionWidth;
            set { if (this.sectionWidth != value) { this.sectionWidth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionWidth))); } }
        }

        private int sectionHeight = 1;
        public int SectionHeight
        {
            get => this.sectionHeight;
            set { if (this.sectionHeight != value) { this.sectionHeight = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SectionHeight))); } }
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

        public ImageCropConfig()
        {
            InitializeComponent();
        }
    }
}
