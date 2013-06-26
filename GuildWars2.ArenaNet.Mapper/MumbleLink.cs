using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GuildWars2.ArenaNet.Mapper
{
    public class MumbleLink : IDisposable
    {
        private uint m_LastTick = 0;
        private bool m_Disposed = false;

        private IntPtr m_Handle;
        private IntPtr m_LinkedMem;

        private LinkedMem Link
        { get { return (LinkedMem)Marshal.PtrToStructure(m_LinkedMem, typeof(LinkedMem)); } }

        public bool DataAvailable
        {
            get
            {
                uint uiTick = Link.uiTick;

                if (uiTick > m_LastTick)
                {
                    m_LastTick = uiTick;
                    return true;
                }

                return false;
            }
        }

        public string GameName
        { get { return Link.name; } }

        public string PlayerName
        { get { return Link.identity; } }

        public int Server
        { get { return Link.context[18]; } }

        public int Map
        { get { return Link.context[14]; } }

        public float PositionX
        { get { return Link.fAvatarPosition[0]; } }

        public float PositionY
        { get { return Link.fAvatarPosition[1]; } }

        public float PositionZ
        { get { return Link.fAvatarPosition[2]; } }

        public int RotationPlayer
        {
            get
            {
                LinkedMem lm = Link;
                return -(int)(Math.Atan2(lm.fAvatarFront[2], lm.fAvatarFront[0]) * 180 / Math.PI) % 360;
            }
        }

        public int RotationCamera
        {
            get
            {
                LinkedMem lm = Link;
                return -(int)(Math.Atan2(lm.fCameraFront[2], lm.fCameraFront[0]) * 180 / Math.PI) % 360;
            }
        }

        public MumbleLink()
        {
            uint linkedMemBytes = (uint)Marshal.SizeOf(typeof(LinkedMem));
            string linkedName = "MumbleLink";

            m_Handle = OpenFileMapping(FileMapAccess.FileMapAllAccess, false, linkedName);
            if (m_Handle == IntPtr.Zero)
            {
                m_Handle = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, FileMapProtection.PageReadWrite, 0, linkedMemBytes, linkedName);
                if (m_Handle == IntPtr.Zero)
                    throw new Exception("Unable to create mumble link to GuildWars2");
            }

            m_LinkedMem = MapViewOfFile(m_Handle, FileMapAccess.FileMapAllAccess, 0, 0, linkedMemBytes);
            if (m_LinkedMem == IntPtr.Zero)
                throw new Exception("Unable to map view of mumble link");
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                CloseHandle(m_Handle);
                m_Handle = IntPtr.Zero;

                m_LinkedMem = IntPtr.Zero;

                m_Disposed = true;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFileMapping(
                IntPtr hFile,
                IntPtr lpFileMappingAttributes,
                FileMapProtection flProtect,
                uint dwMaximumSizeHigh,
                uint dwMaximumSizeLow,
                [MarshalAs(UnmanagedType.LPStr)] string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenFileMapping(
                FileMapAccess dwDesiredAccess,
                bool bInheritHandle,
                [MarshalAs(UnmanagedType.LPStr)] string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr MapViewOfFile(
                IntPtr hFileMappingObject,
                FileMapAccess dwDesiredAccess,
                UInt32 dwFileOffsetHigh,
                UInt32 dwFileOffsetLow,
                UInt32 dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern Boolean CloseHandle(IntPtr handle);

        [Flags]
        private enum FileMapProtection : uint
        {
            PageReadonly = 0x02,
            PageReadWrite = 0x04,
            PageWriteCopy = 0x08,
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40
        }

        [Flags]
        private enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            FileMapExecute = 0x0020
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LinkedMem
        {
            public UInt32 uiVersion;
            public UInt32 uiTick;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] fAvatarPosition;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] fAvatarFront;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] fAvatarTop;
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string name;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] fCameraPosition;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] fCameraFront;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] fCameraTop;
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string identity;

            public UInt32 context_len;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public short[] context;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
            public string description;
        }
    }
}
