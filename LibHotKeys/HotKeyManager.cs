using System;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace LibHotKeys
{
    public static class HotKeyManager
    {
        public static event Action<HotKeyEventArgs> HotKeyPressed;

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
                Window.HotKeyPressed += (e) => HotKeyPressed?.Invoke(e);
                WindowReadyEvent.Set();
                Application.Run(Window);
            }, TaskCreationOptions.LongRunning);
        }

        public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
        {
            WindowReadyEvent.WaitOne();
            int id = Interlocked.Increment(ref CurrentId);
            Window.Invoke(new RegisterHotKeyDelegate(WinAPI.RegisterHotKey), WindowPtr, id, (uint)modifiers, (uint)key);
            return id;
        }

        public static void UnregisterHotKey(int id)
            => Window.Invoke(new UnRegisterHotKeyDelegate(WinAPI.UnregisterHotKey), WindowPtr, id);

        public static bool IsKeyPressed(Keys key)
            => 0 != (WinAPI.GetAsyncKeyState((int)key) & 0x8000);
    }
}