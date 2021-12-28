using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using KeypadDriverV2.IO;

namespace KeypadDriverV2
{
    public class KeypadDriver : IDisposable
    {
        private bool disposed = false;

        public void Run()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Keypad Driver V2",
                Flags = ContextFlags.ForwardCompatible
            };

            using (var window = new KeypadDriverWindow(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
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

                disposed = true;
            }
        }
    }
}
