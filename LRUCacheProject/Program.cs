using System;
using System.Collections.Generic;
using System.Threading;
using LRUCacheProject.CacheNotificationHelpers;

namespace LRUCacheProject
{
    class Program
    {
        static void Main()
        {
            // Create or get instance of LRUCache
            LRUCache lruCache = LRUCache.LRUCacheInstance;

            // Create a cache node eviction subscriber instance
            CacheNodeEvictionSubscriber cacheNodeEvictionSubscriber = new CacheNodeEvictionSubscriber();

            // Subscribe to get notified when cache nodes are evicted
            cacheNodeEvictionSubscriber.Subsribe(lruCache);

            // find out when cache is
            lruCache.SetCacheCapacity(2, allowEviction: true);
            lruCache.AddOrMoveLinkedListCacheNode(67.2, "Michael Kane");
            lruCache.AddOrMoveLinkedListCacheNode("2", 2);
            lruCache.AddOrMoveLinkedListCacheNode("3", 3);

            // Unsubscribe
            cacheNodeEvictionSubscriber.Unsubsribe(lruCache);

            // do the same
            lruCache.AddOrMoveLinkedListCacheNode(true, "bolocks");
            lruCache.AddOrMoveLinkedListCacheNode("5", 5);
            lruCache.AddOrMoveLinkedListCacheNode("6", 6);

            // Subscribe again
            cacheNodeEvictionSubscriber.Subsribe(lruCache);

            // do the same
            lruCache.AddOrMoveLinkedListCacheNode("7", 7);
            lruCache.AddOrMoveLinkedListCacheNode("8", 8);
            lruCache.AddOrMoveLinkedListCacheNode("9", 9);

            // Expected print outs:
            // 1
            // 5
            // 6
            // 7
        }
    }
}
