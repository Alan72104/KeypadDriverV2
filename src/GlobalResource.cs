using KeypadDriverV2.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeypadDriverV2
{
    public static class GlobalResource
    {
        public static readonly ResourceStore Resources = new NamespacedResourceStore(new EmbeddedResourceStore(typeof(KeypadDriver).Assembly), "KeypadDriverV2.src.Resources");

        public static readonly ResourceStore Shaders = new NamespacedResourceStore(Resources, "Shaders");
    }
}
