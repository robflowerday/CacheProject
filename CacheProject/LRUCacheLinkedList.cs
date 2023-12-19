using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject
{
    public class LRUCache<TKey, TCacheNodeValue>
    {
        //private CacheNode<TKey, TValue>? headNode = null;
        //private CacheNode<TKey, TValue>? tailNode = null;

        private readonly int cacheCapacity;
        private Dictionary<TKey, TCacheNodeValue> cacheDictionary;

        public LRUCache(int cacheCapacity)
        {
            this.cacheCapacity = cacheCapacity;
            this.cacheDictionary = new Dictionary<>(cacheCapacity);
        }

        public void AddOrMoveCacheNode(TKey pCacheNodeKey, TCacheNodeValue pCacheNodeValue)
        {
            // Check if item is in cache
            if 
            // If item is in Dictionary
                // Move item to front of DoublyLinkedList
            // Else
                // If at capacity limit
                    // Determine key of tail node
                    // Remove Dictionary value with this Key
                    // Remove tail node from DoublyLinkedList
                // Add item to Dictionary
                // Add item to front of DoublyLinkedList
        }

        public TValue GetCacheNodeValue(TKey pCacheNodeKey)
        {
            // If key is in Dictionary
                // Return value from dictionary
            // Else
                // Return null
        }
    }
}
