using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject
{
    public class LRUCache<TCacheNodeKey, TCacheNodeValue>
    {
        private readonly int cacheCapacity;
        private Dictionary<TCacheNodeKey, CacheNode<TCacheNodeKey, TCacheNodeValue>> cacheDictionary;
        private DoublyLinkedList<TCacheNodeKey, TCacheNodeValue> cacheDoublyLinkedList;

        public LRUCache(int cacheCapacity)
        {
            this.cacheCapacity = cacheCapacity;
            this.cacheDictionary = new Dictionary<TCacheNodeKey, CacheNode<TCacheNodeKey, TCacheNodeValue>>(cacheCapacity);
            this.cacheDoublyLinkedList = new DoublyLinkedList<TCacheNodeKey, TCacheNodeValue>();
        }

        public void AddOrMoveLinkedListCacheNode(TCacheNodeKey pCacheNodeKey, TCacheNodeValue pCacheNodeValue)
        {
            // Check if item is in cache
            if (cacheDictionary.ContainsKey(pCacheNodeKey))
            {
                // Get the Node
                CacheNode<TCacheNodeKey, TCacheNodeValue> cacheNodeToMove = cacheDictionary[pCacheNodeKey];

                // Move item to front of DoublyLinkedList
                cacheDoublyLinkedList.MoveNodeToHeadOfList(cacheNodeToMove);
            }
            else
            {
                // If at capacity limit
                if (this.cacheDictionary.Count >= this.cacheCapacity)
                {
                    // Determine key of tail node
                    TCacheNodeKey tailNodeKey = cacheDoublyLinkedList.Tail.CacheNodeKey;

                    // Remove Dictionary value with this Key
                    cacheDictionary.Remove(tailNodeKey);

                    // Remove tail node from DoublyLinkedList
                    CacheNode<TCacheNodeKey, TCacheNodeValue>? evictedNode = cacheDoublyLinkedList.EvictLRUNode();
                }
                // Create new node to add
                CacheNode<TCacheNodeKey, TCacheNodeValue> newCacheNode = new CacheNode<TCacheNodeKey, TCacheNodeValue>(pCacheNodeKey, pCacheNodeValue);
                
                // Add item to Dictionary
                cacheDictionary.Add(pCacheNodeKey, newCacheNode);

                // Add item to front of DoublyLinkedList
                cacheDoublyLinkedList.AddAsHead(newCacheNode);
            }
        }

        public TCacheNodeValue? GetCacheNodeValue(TCacheNodeKey pCacheNodeKey)
        {
            if (cacheDictionary.ContainsKey(pCacheNodeKey))
                return cacheDictionary[pCacheNodeKey].CacheNodeValue;
            return default(TCacheNodeValue);
        }
    }
}
