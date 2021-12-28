using System;
using System.IO;
using System.Reflection;

namespace KeypadDriverV2.IO
{
    public class EmbeddedResourceStore : IResourceStore
    {
        private readonly Assembly assembly;

        public EmbeddedResourceStore(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public byte[] Get(string name)
        {
            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    return null;
                byte[] res = new byte[stream.Length];
                stream.Read(res, 0, res.Length);
                return res;
            }
        }
    }
}
