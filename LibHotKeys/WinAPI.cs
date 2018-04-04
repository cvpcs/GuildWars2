using System;
using System.Runtime.InteropServices;

namespace LibHotKeys
{
    internal class WinAPI
    {
        [DllImport("user32", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32", SetLastError = true)]
        public static extern ushort GetAsyncKeyState(int vKey);
    }
}
