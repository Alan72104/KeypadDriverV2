using System.Collections.Generic;

namespace KeypadDriverV2.IO
{
    public class ResourceStore : IResourceStore
    {
        private readonly List<IResourceStore> stores = new();

        public ResourceStore(IResourceStore store)
        {
            stores.Add(store);
        }

        public ResourceStore(IResourceStore[] stores)
        {
            this.stores.AddRange(stores);
        }

        public virtual byte[] Get(string name)
        {
            byte[] res;

            foreach (IResourceStore store in stores)
            {
                res = store.Get(name);
                if (res != null)
                    return res;
            }

            return null;
        }

        public IResourceStore GetNamespacedStoreOf(string prefix) => new NamespacedResourceStore(this, prefix);
    }
}
