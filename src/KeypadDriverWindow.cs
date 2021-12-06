using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KeypadDriverV2.Win32;

namespace KeypadDriverV2
{
    public class KeypadDriverWindow : IDisposable
    {
        private const int ERROR_CLASS_ALREADY_EXISTS = 1410;

        private bool disposed;
        private IntPtr hWnd;
        private WndProc wndProc;
        public IntPtr HWnd { get => hWnd; }
        private int width;
        private int height;

        public KeypadDriverWindow(IntPtr hIcon, int width, int height)
        {
            wndProc = CustomWndProc;
            string name = "KeypadDriver";
            this.width = width;
            this.height = height;

            WndClass wndClass = new WndClass();
            wndClass.ClassName = name;
            wndClass.HIcon = hIcon;
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
                                  width,
                                  height,
                                  IntPtr.Zero,
                                  IntPtr.Zero,
                                  IntPtr.Zero,
                                  IntPtr.Zero);
        }

        public void ShowWindow()
        {
            AnimateWindow(hWnd, 1000, AnimateWindowEnums.Blend);
        }

        public void HideWindow()
        {
            AnimateWindow(hWnd, 200, AnimateWindowEnums.Blend | AnimateWindowEnums.Hide);
        }

        private IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            System.Diagnostics.Debug.Print($"Received message {Enum.GetName((WM)msg) ?? msg.ToString("X4")} {wParam.ToString("X4")} {lParam.ToString("X4")}");
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
                    DestroyWindow(hWnd);

                disposed = true;
            }
        }
    }
}
