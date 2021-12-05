using System;
using System.Runtime.InteropServices;
using static KeypadDriverV2.Win32;

namespace KeypadDriverV2
{
    public static class Win32
    {
#pragma warning disable CA1401 // P/Invokes should not be visible
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct NotifyIconData
        {
            [FieldOffset(0)] public int Size;
            [FieldOffset(4)] public IntPtr HWnd;
            [FieldOffset(8)] public int ID;
            [FieldOffset(12)] public NotifyIconFlags Flags;
            [FieldOffset(16)] public int CallbackMessage;
            [FieldOffset(20)] public IntPtr Icon;
            [FieldOffset(24), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string Tip;
            [FieldOffset(280)] public NotifyIconStates State;
            [FieldOffset(284)] public NotifyIconStates StateMask;
            [FieldOffset(288), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Info;
            [FieldOffset(800)] public int Timeout;
            [FieldOffset(800)] public int Version;
            [FieldOffset(808), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string InfoTitle;
            [FieldOffset(936)] public NotifyIconInfoFlags InfoFlags;
            [FieldOffset(940)] public GUID GuidItem;
            [FieldOffset(944)] public IntPtr HBalloonIcon;
        }

        [Flags]
        public enum NotifyIconFlags
        {
            Message = 0x1,
            Icon = 0x2,
            Tip = 0x4,
            State = 0x8,
            Info = 0x10,
            Guid = 0x20,
            Realtime = 0x40,
            ShowTip = 0x80
        }

        [Flags]
        public enum NotifyIconStates
        {
            Hidden = 0x1,
            SharedIcon = 0x2
        }

        [Flags]
        public enum NotifyIconInfoFlags
        {
            None = 0x0,
            Info = 0x1,
            Warning = 0x2,
            Error = 0x3,
            User = 0x4,
            NoSound = 0x10,
            LargeIcon = 0x20,
            RespectQuietTime = 0x80,
            IconMask = 0xF

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GUID
        {
            int Data1;
            short Data2;
            short Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            short[] Data4;
        }

        public enum NotifyIconMessage
        {
            Add = 0x0,
            Modify = 0x1,
            Delete = 0x2,
            SetFocus = 0x3,
            SetVersion = 0x4
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Shell_NotifyIcon(NotifyIconMessage message, ref NotifyIconData data);

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public const int WM_APP = 0x8000;
        public const int WMAPP_NOTIFYCALLBACK = WM_APP + 1;
        public const int WMAPP_HIDEFLYOUT = WM_APP + 2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WndClass
        {
            public uint Style;
            public IntPtr FnWndProc;
            public int ClsExtra;
            public int WndExtra;
            public IntPtr HInstance;
            public IntPtr HIcon;
            public IntPtr HCursor;
            public IntPtr HBrBackground;
            [MarshalAs(UnmanagedType.LPWStr)] public string MenuName;
            [MarshalAs(UnmanagedType.LPWStr)] public string ClassName;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern ushort RegisterClass(ref WndClass wndClass);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(int exStyle,
                                                   [MarshalAs(UnmanagedType.LPWStr)] string className,
                                                   [MarshalAs(UnmanagedType.LPWStr)] string windowName,
                                                   int style,
                                                   int x,
                                                   int y,
                                                   int width,
                                                   int height,
                                                   IntPtr hWndParent,
                                                   IntPtr hMenu,
                                                   IntPtr hInstance,
                                                   IntPtr param);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);
#pragma warning restore CA1401 // P/Invokes should not be visible
    }

    public class Window : IDisposable
    {
        private const int ERROR_CLASS_ALREADY_EXISTS = 1410;

        private bool disposed;
        private IntPtr hWnd;
        private WndProc wndProc;
        public IntPtr HWnd { get => hWnd; }
        public WndProc WndProc { get => wndProc; set => wndProc = value; }

        public Window(string name)
        {
            if (wndProc == null)
                wndProc = CustomWndProc;

            WndClass wndClass = new WndClass();
            wndClass.ClassName = name;
            wndClass.FnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);

            ushort atom = RegisterClass(ref wndClass);

            int lastError = Marshal.GetLastWin32Error();

            if (atom == 0 && lastError != ERROR_CLASS_ALREADY_EXISTS)
            {
                throw new Exception("Cannot register window class");
            }

            hWnd = CreateWindowEx(0,
                                  name,
                                  string.Empty,
                                  0,
                                  0,
                                  0,
                                  0,
                                  0,
                                  IntPtr.Zero,
                                  IntPtr.Zero,
                                  IntPtr.Zero,
                                  IntPtr.Zero);
        }

        public virtual IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Managed
                if (disposing)
                {
                }

                // Unmanaged
                if (hWnd != IntPtr.Zero)
                {
                    DestroyWindow(hWnd);
                    hWnd = IntPtr.Zero;
                }

                disposed = true;
            }
        }
    }
}
