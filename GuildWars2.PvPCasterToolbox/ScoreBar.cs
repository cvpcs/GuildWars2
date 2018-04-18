using System;
using System.Drawing;

namespace GuildWars2.PvPCasterToolbox
{
    internal class ScoreBar
    {
        private readonly string title;
        private readonly string backgroundBarImagePath;
        private readonly string boostBarImagePath;
        private readonly string scoreBarImagePath;
        private readonly bool isFlipped;

        public event Action<RectangleF> PositionChanged;

        public ScoreBarWindow Window { get; private set; }

        private bool isOverlayMode = true;
        public bool IsOverlayMode
        {
            get => this.isOverlayMode;
            set
            {
                if (this.isOverlayMode != value)
                {
                    this.isOverlayMode = value;
                    Window.Close();
                    Window = this.CreateWindow(value, Window.GetWindowRect());
                    Window.Show();
                }
            }
        }

        public ScoreBar(string title,
                        string backgroundBarImagePath, string boostBarImagePath, string scoreBarImagePath,
                        bool isFlipped = false)
        {
            this.title = title;
            this.backgroundBarImagePath = backgroundBarImagePath;
            this.boostBarImagePath = boostBarImagePath;
            this.scoreBarImagePath = scoreBarImagePath;
            this.isFlipped = isFlipped;

            Window = this.CreateWindow(this.isOverlayMode);
        }

        private ScoreBarWindow CreateWindow(bool overlayMode, RectangleF? position = null)
        {
            var window = new ScoreBarWindow(this.backgroundBarImagePath,
                                            this.boostBarImagePath,
                                            this.scoreBarImagePath,
                                            this.isFlipped,
                                            overlayMode,
                                            position) { Title = this.title };

            window.LocationChanged += (s, e) => this.PositionChanged?.Invoke(Window.GetWindowRect());
            window.SizeChanged += (s, e) => this.PositionChanged?.Invoke(Window.GetWindowRect());

            return window;
        }
    }
}
