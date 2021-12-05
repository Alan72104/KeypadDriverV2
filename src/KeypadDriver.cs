using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static KeypadDriverV2.Win32;

namespace KeypadDriverV2
{
    public class KeypadDriver : IDisposable
    {
        private bool disposed = false;
        private NotifyIconData notifyIconData;
        private Window window;

        public void Run()
        {
            window = new Window("keypadDriver")
            {
                WndProc = (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) =>
                {
                    Console.WriteLine($"Received message {msg.ToString("X4")} {wParam.ToString("X4")} {lParam.ToString("X4")}");
                    return DefWindowProc(hWnd, msg, wParam, lParam);
                }
            };

            notifyIconData = new NotifyIconData
            {
                Size = Marshal.SizeOf<NotifyIconData>(),
                HWnd = window.HWnd,
                ID = 0,
                Flags = NotifyIconFlags.Message | NotifyIconFlags.Icon | NotifyIconFlags.Tip | NotifyIconFlags.ShowTip,
                Tip = "KeypadDriver",
                CallbackMessage = WMAPP_NOTIFYCALLBACK
            };

            if (Shell_NotifyIcon(NotifyIconMessage.Add, ref notifyIconData) == false)
                throw new Exception("Cannot create notify icon");

            Thread.Sleep(10000);
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
                    window.Dispose();
                    window = null;
                }

                // Unmanaged
                Shell_NotifyIcon(NotifyIconMessage.Delete, ref notifyIconData);

                disposed = true;
            }
        }
    }
}
