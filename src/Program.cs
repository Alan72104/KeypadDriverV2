using System;

namespace KeypadDriverV2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (KeypadDriver keypadDriver = new KeypadDriver())
                keypadDriver.Run();
        }
    }
}
