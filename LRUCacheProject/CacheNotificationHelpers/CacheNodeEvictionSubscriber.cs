using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRUCacheProject.CacheNotificationHelpers
{
    /// <summary>
    /// Creates event handler for cache node eviction and enables a consumer to
    /// subscribe and unsucribe from notifications.
    /// </summary>
    public class CacheNodeEvictionSubscriber
    {
        public void Subsribe(LRUCache lruCache)
        {
            // Add event handler to LRUCache eciction event
            lruCache.CacheNodeEviction += HandleCacheNodeEviction;
        }

        public void Unsubsribe(LRUCache lruCache)
        {
            // Remove event handler to LRUCache eciction event
            lruCache.CacheNodeEviction -= HandleCacheNodeEviction;
        }

        /// <summary>
        /// Subscriber Event Handler for CacheNodeEviction event
        /// </summary>
        /// <param name="eventSendingObject"> The object that triggered the event. </param>
        /// <param name="eventArgs"> The details passed through from the event. </param>
        private void HandleCacheNodeEviction(object eventSendingObject, CacheNodeEvictionEventArgs eventArgs)
        {
            // Display details about the evicted node
            Console.WriteLine("Node evicted:");
            Console.WriteLine($"Node Key: {eventArgs.cacheNodeKey}");
            Console.WriteLine($"Node Value: {eventArgs.cacheNodeValue}");
            Console.WriteLine($"Node Eviction Time: {eventArgs.dateTimeEvicted}");
        }
    }
}
