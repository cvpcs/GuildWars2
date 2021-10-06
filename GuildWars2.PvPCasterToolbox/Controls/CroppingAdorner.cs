using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ContentAlignment = System.Drawing.ContentAlignment;

namespace GuildWars2.PvPCasterToolbox.Controls
{
    internal class CroppingAdorner : Adorner
    {
        private const int THUMB_WIDTH = 6;
        private static readonly IDictionary<ContentAlignment, Rect> DragDeltaRectMaskMap = new Dictionary<ContentAlignment, Rect>
        {
            [ContentAlignment.TopCenter]    = new Rect(0, 1,  0, -1),
            [ContentAlignment.BottomCenter] = new Rect(0, 0,  0,  1),
            [ContentAlignment.MiddleLeft]   = new Rect(1, 0, -1,  0),
            [ContentAlignment.MiddleRight]  = new Rect(0, 0,  1,  0),
            [ContentAlignment.TopLeft]      = new Rect(1, 1, -1, -1),
            [ContentAlignment.TopRight]     = new Rect(0, 1,  1, -1),
            [ContentAlignment.BottomLeft]   = new Rect(1, 0, -1,  1),
            [ContentAlignment.BottomRight]  = new Rect(0, 0,  1,  1),
        };

        private IDictionary<ContentAlignment, CropThumb> thumbs = new Dictionary<ContentAlignment, CropThumb>
        {
            [ContentAlignment.TopCenter] = new CropThumb(THUMB_WIDTH, Cursors.SizeNS),
            [ContentAlignment.BottomCenter] = new CropThumb(THUMB_WIDTH, Cursors.SizeNS),
            [ContentAlignment.MiddleLeft] = new CropThumb(THUMB_WIDTH, Cursors.SizeWE),
            [ContentAlignment.MiddleRight] = new CropThumb(THUMB_WIDTH, Cursors.SizeWE),
            [ContentAlignment.TopLeft] = new CropThumb(THUMB_WIDTH, Cursors.SizeNWSE),
            [ContentAlignment.TopRight] = new CropThumb(THUMB_WIDTH, Cursors.SizeNESW),
            [ContentAlignment.BottomLeft] = new CropThumb(THUMB_WIDTH, Cursors.SizeNESW),
            [ContentAlignment.BottomRight] = new CropThumb(THUMB_WIDTH, Cursors.SizeNWSE)
        };

        private Canvas thumbCanvas;
        private VisualCollection visualCollection;

        public CroppingAdorner(UIElement adornedElement, Rect initialCroppingRect)
            : base(adornedElement)
        {
            this.visualCollection = new VisualCollection(this);

            this.thumbCanvas = new Canvas
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            this.visualCollection.Add(this.thumbCanvas);

            foreach (var kvp in this.thumbs)
            {
                this.thumbCanvas.Children.Add(kvp.Value);
                kvp.Value.DragDelta += (sender, args) =>
                {
                    if (sender is CropThumb &&
                        DragDeltaRectMaskMap.ContainsKey(kvp.Key))
                    {
                        this.ThumbDragDelta(DragDeltaRectMaskMap[kvp.Key], args.HorizontalChange, args.VerticalChange);
                    }
                };
            }
        }

        protected override int VisualChildrenCount => this.visualCollection.Count;
        protected override Visual GetVisualChild(int index) => this.visualCollection[index];

        private void ThumbDragDelta(Rect deltaMask, double deltaX, double deltaY)
        {
            Rect rect = new Rect(); // define from somewhere

            if (rect.Width + deltaMask.Width * deltaX < 0)
            {
                deltaX = -rect.Width / deltaMask.Width;
            }

            if (rect.Height + deltaMask.Height * deltaY < 0)
            {
                deltaY = -rect.Height / deltaMask.Height;
            }

            rect = new Rect(rect.Left + deltaMask.Left * deltaX,
                            rect.Top + deltaMask.Top * deltaY,
                            rect.Width + deltaMask.Width * deltaX,
                            rect.Height + deltaMask.Height * deltaY);
        }

        private class CropThumb : Thumb
        {
            private readonly Size size;

            public CropThumb(Size size, Cursor cursor = null)
            {
                this.size = size;
                Cursor = cursor ?? Cursors.Arrow;
            }

            public CropThumb(int width, int height, Cursor cursor = null)
                : this(new Size(width, height), cursor)
            { }

            public CropThumb(int size, Cursor cursor = null)
                : this(size, size, cursor)
            { }

            protected override Visual GetVisualChild(int index)
                => null;

            protected override void OnRender(DrawingContext drawingContext)
                => drawingContext.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), new Rect(this.size), 1, 1);

            public void SetPosition(double x, double y)
            {
                Canvas.SetTop(this, y - this.size.Height / 2);
                Canvas.SetLeft(this, x - this.size.Width / 2);
            }
        }
    }
}
