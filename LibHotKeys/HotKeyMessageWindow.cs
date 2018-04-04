using System;
using System.Windows.Forms;

namespace LibHotKeys
{
    internal class HotKeyMessageWindow : Form
    {
        public event Action<HotKey> HotKeyPressed;

        private const int WM_HOTKEY = 0x312;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                HotKeyPressed?.Invoke(new HotKey(m.LParam));
            }

            base.WndProc(ref m);
        }

        // Ensure the window never becomes visible
        protected override void SetVisibleCore(bool value)
            => base.SetVisibleCore(false);
    }
}
