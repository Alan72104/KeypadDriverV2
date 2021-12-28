namespace KeypadDriverV2.IO
{
    public class NamespacedResourceStore : ResourceStore
    {
        private readonly string prefix;

        public NamespacedResourceStore(IResourceStore store, string prefix) : base(store)
        {
            this.prefix = prefix;
        }

        public override byte[] Get(string name) => base.Get(prefix + '.' + name);
    }
}
