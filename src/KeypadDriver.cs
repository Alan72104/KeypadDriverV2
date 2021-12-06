using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Image = System.Drawing.Image;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static KeypadDriverV2.Win32;
using System.Reflection;
using System.IO;

namespace KeypadDriverV2
{
    public class KeypadDriver : IDisposable
    {
        private bool disposed = false;
        private NotifyIconData notifyIconData;
        private KeypadDriverWindow window;
        private Image windowIcon;
        private IntPtr hWindowIcon;

        public void Run()
        {
            LoadResources();
            window = new KeypadDriverWindow(hWindowIcon, 750, 500);

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

            window.ShowWindow();

            Thread.Sleep(10000);
        }

        private void LoadResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            using (Stream stream = assembly.GetManifestResourceStream(resourceNames.Single(s => s.EndsWith("Icon.ico"))))
                windowIcon = Image.FromStream(stream);
            hWindowIcon = ((Bitmap)windowIcon).GetHicon();
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
                    windowIcon.Dispose();
                }

                // Unmanaged
                Shell_NotifyIcon(NotifyIconMessage.Delete, ref notifyIconData);
                DestroyIcon(hWindowIcon);

                disposed = true;
            }
        }
    }
}
