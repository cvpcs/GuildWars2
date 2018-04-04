using System;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LibHotKeys
{
    public static class HotKeyManager
    {
        private const uint KEYMODIFIER_NOREPEAT = 0x4000;

        public static event Action<HotKey> HotKeyPressed;

        private delegate bool RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
        private delegate bool UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

        private static int CurrentId = 0;
        private static volatile HotKeyMessageWindow Window;
        private static volatile IntPtr WindowPtr;
        private static ManualResetEvent WindowReadyEvent = new ManualResetEvent(false);

        static HotKeyManager()
        {
            Task.Factory.StartNew(() =>
            {
                Window = new HotKeyMessageWindow();
                WindowPtr = Window.Handle;
                Window.HotKeyPressed += (hotKey) => HotKeyPressed?.Invoke(hotKey);
                WindowReadyEvent.Set();
                Application.Run(Window);
            }, TaskCreationOptions.LongRunning);
        }

        public static int RegisterHotKey(HotKey hotKey, bool noRepeat = true)
            => RegisterHotKey(hotKey.Keys, hotKey.Modifiers, noRepeat);

        public static int RegisterHotKey(Key key, ModifierKeys modifiers = ModifierKeys.None, bool noRepeat = true)
            => RegisterHotKey((Keys)KeyInterop.VirtualKeyFromKey(key), modifiers, noRepeat);

        public static int RegisterHotKey(Keys key, ModifierKeys modifiers = ModifierKeys.None, bool noRepeat = true)
        {
            WindowReadyEvent.WaitOne();
            int id = Interlocked.Increment(ref CurrentId);
            Window.Invoke(new RegisterHotKeyDelegate(WinAPI.RegisterHotKey), WindowPtr, id,
                                                     (uint)modifiers | (noRepeat ? KEYMODIFIER_NOREPEAT : 0),
                                                     (uint)key);
            return id;
        }

        public static void UnregisterHotKey(int id)
            => Window.Invoke(new UnRegisterHotKeyDelegate(WinAPI.UnregisterHotKey), WindowPtr, id);

        public static bool IsKeyPressed(Key key)
            => IsKeyPressed((Keys)KeyInterop.VirtualKeyFromKey(key));

        public static bool IsKeyPressed(Keys key)
            => 0 != (WinAPI.GetAsyncKeyState((int)key) & 0x8000);
    }
}