using System;
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

        private uint imageWidth = 1;
        public uint ImageWidth
        {
            get => this.imageWidth;
            set
            {
                if (this.imageWidth != value && value > 0)
                {
                    this.imageWidth = value;

                    CropX = Math.Min(CropX, this.imageWidth - 1);
                    CropWidth = Math.Min(CropWidth, this.imageWidth - CropX);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageWidth)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropXMaximum)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropWidthMaximum)));
                }
            }
        }

        private uint imageHeight = 1;
        public uint ImageHeight
        {
            get => this.imageHeight;
            set
            {
                if (this.imageHeight != value && value > 0)
                {
                    this.imageHeight = value;

                    CropY = Math.Min(CropY, this.imageHeight - 1);
                    CropHeight = Math.Min(CropHeight, this.imageHeight - CropY);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageHeight)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropYMaximum)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropHeightMaximum)));
                }
            }
        }

        public uint CropXMinimum => 0;
        public uint CropYMinimum => 0;
        public uint CropWidthMinimum => 1;
        public uint CropHeightMinimum => 1;

        public uint CropXMaximum => ImageWidth - CropWidthMinimum;
        public uint CropYMaximum => ImageHeight - CropHeightMinimum;
        public uint CropWidthMaximum => ImageWidth;
        public uint CropHeightMaximum => ImageHeight;

        private uint cropX = 0;
        public uint CropX
        {
            get => this.cropX;
            set
            {
                if (this.cropX != value)
                {
                    this.cropX = value;
                    CropWidth = Math.Min(CropWidth, ImageWidth - value);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropX)));
                }
            }
        }

        private uint cropY = 0;
        public uint CropY
        {
            get => this.cropY;
            set
            {
                if (this.cropY != value)
                {
                    this.cropY = value;
                    CropHeight = Math.Min(CropHeight, ImageHeight - value);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropY)));
                }
            }
        }

        private uint cropWidth = 1;
        public uint CropWidth
        {
            get => this.cropWidth;
            set
            {
                if (this.cropWidth != value)
                {
                    this.cropWidth = value;
                    CropX = Math.Min(CropX, ImageWidth - value);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropWidth)));
                }
            }
        }

        private uint cropHeight = 1;
        public uint CropHeight
        {
            get => this.cropHeight;
            set
            {
                if (this.cropHeight != value)
                {
                    this.cropHeight = value;
                    CropY = Math.Min(CropY, ImageHeight - value);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropHeight)));
                }
            }
        }

        public Rectangle CropRect
        {
            get => new Rectangle((int)CropX, (int)CropY, (int)CropWidth, (int)CropHeight);
            set
            {
                CropX = (uint)value.X;
                CropY = (uint)value.Y;
                CropWidth = (uint)value.Width;
                CropHeight = (uint)value.Height;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CropRect)));
            }
        }

        public ImageCropConfig()
            => InitializeComponent();
    }
}
