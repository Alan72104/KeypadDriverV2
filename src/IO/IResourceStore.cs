using System.Collections.Generic;

namespace KeypadDriverV2.IO
{
    public interface IResourceStore
    {
        byte[] Get(string name);
    }
}
