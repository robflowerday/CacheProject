using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject
{
    public class LRUCache<TCacheNodeKey, TCacheNodeValue>
    {
        private Dictionary<TCacheNodeKey, CacheNode<TCacheNodeKey, TCacheNodeValue>> cacheDictionary;

        public LRUCache(int cacheCapacity)
        {
            this.cacheCapacity = cacheCapacity;
            this.cacheDictionary = new Dictionary<>(cacheCapacity);
        }

    }
}
