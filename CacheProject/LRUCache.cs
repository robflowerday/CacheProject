using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using CacheProject.CacheNotificationHelpers;
using CacheProject.DataStructureHelpers;

namespace CacheProject
{
    /// <summary>
    /// A LRU Cache implementation allowing for generic key and value types using a Dictionary
    /// and Doubly Linked List undeer the hood, performing all common operations in O(1) time
    /// complexity and implementing the singleton design pattern as well as offering consumers
    /// the ability to be notified when a cache node is evicted.
    /// </summary>
    public class LRUCache
    {
        /// <summary>
        /// Event for notifying consumers when items are evicted.
        /// </summary>
        public event EventHandler<CacheNodeEvictionEventArgs> CacheNodeEviction;

        /// <summary>
        /// Handle event when event arguments are passed by the event trigger.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void OnCacheNodeEviction(CacheNodeEvictionEventArgs eventArgs)
        {
            EventHandler<CacheNodeEvictionEventArgs> eventHandler = CacheNodeEviction;
            if (eventHandler != null)
                eventHandler(this, eventArgs);
        }

        // Instance of cache to enable use of LRUCache in a singleton pattern
        private static LRUCache? lruCacheInstance;

        // Lock object used to ensure thread safety that distinguishes between reading
        // and writing for better performance when only reading takes place and can be
        // done simultaneously.
        private ReaderWriterLockSlim readWriteLockObject = new ReaderWriterLockSlim();

        // lock object for creating a single instance of a cache amd for when
        // differenciating between reading and writing is not neccessary.
        private static readonly object lockInstanceObject = new object();

        // Next 3 data structures here are static as there is only one intended use of this class.
        private static int cacheCapacity;

        // Cache item lookups are performed in O(1) time complexity due to the dictionary
        private static Dictionary<object, CacheNode> cacheDictionary;
        private static DoublyLinkedList cacheDoublyLinkedList;

        public int CacheCapacity { get; }

        /// <summary>
        /// Set cache capacity, written in separate method rather than setter above to allow input parameters.
        /// 
        /// Requires manual asertion that eviction is allowed when resizing down would evict current cache members.
        /// </summary>
        /// <param name="newCacheCapacity"> Number of items to allow in cache after change. </param>
        /// <param name="allowEviction"> Determine if the cache resizing should allow for eviction of cache items.
        /// Default to false, require manual setting to avoid accidental deletion of other users cache items. </param>
        public void SetCacheCapacity(int newCacheCapacity, bool allowEviction=false)
        {
            // Don't allow a negative or 0 capacity
            if (newCacheCapacity <= 0)
                throw new ArgumentException("Cache capacity must be greater than 0.");

            // Don't allow eviction of nodes unless explicity instructed
            if (allowEviction == false && newCacheCapacity > cacheDictionary.Count)
                throw new UnauthorizedAccessException("You must set parameter 'allowEviction=true' to enable eviction of existing nodes. Your current attempt to resize the cache capacity to " + newCacheCapacity + " would mean the eviction of " + (cacheDictionary.Count - newCacheCapacity) + " nodes.");

            // Resize cache in thread safe mannor
            readWriteLockObject.EnterWriteLock();
            try
            {
                // Change capacity of cache
                cacheCapacity = newCacheCapacity;

                // Evict nodes if neccessary (this could be optimsed by creating EvictManyLRU method
                // in DoublyLinkedList.cs which does not change pointers to other LinkedList nodes
                // until all but the last eviction is complete. This would require thinking about
                // how the nodes that still have references to them are removed from the heap if
                // they implemented an IDisposable interface, in this implementation memory management
                // for this use case is dealt with by the garbage collector).
                while (cacheDictionary.Count > cacheCapacity)
                {
                    // Remove tail node from LinkedList and corresponding dictionary key, value pair
                    object tailNodeKey = cacheDoublyLinkedList.Tail.CacheNodeKey;
                    cacheDictionary.Remove(tailNodeKey);
                    CacheNode? evictedNode = cacheDoublyLinkedList.EvictLRUNode();

                    // Trigger event to notify subscribed consumers about cache node eviction
                    CacheNodeEvictionEventArgs eventArgs = new CacheNodeEvictionEventArgs();
                    eventArgs.cacheNodeKey = evictedNode.CacheNodeKey;
                    eventArgs.cacheNodeValue = evictedNode.CacheNodeValue;
                    eventArgs.dateTimeEvicted = DateTime.Now;
                    OnCacheNodeEviction(eventArgs);
                }
            }
            finally
            {
                readWriteLockObject.ExitWriteLock();
            }
        }

        // Set accessable Property as instance for the lruCache
        public static LRUCache LRUCacheInstance
        {
            // Using singleton pattern only allow creation of new cache instance
            // if one doesn't already exist.
            get
            {
                // This if stataement improves performance, only locking the object if a new instance may need creating
                if (lruCacheInstance == null)
                {
                    lock (lockInstanceObject)
                    {
                        // null coallease assignment to ensure only one instance is created if multiple
                        // threads enter previous if statement before new instance is created
                        // null coalleasing operator here checks if lruCacheInstance is null and assigns it to the right
                        // side result if it is.
                        lruCacheInstance ??= new LRUCache(chosenCacheCapacity: 100);
                    }
                }
                // If an LRU Cache instance already exists, return it
                return lruCacheInstance;
            }
        }

        /// <summary>
        /// Constructor of LRUCache using a Dictionary and a doubly linked list.
        /// Set to private to enable the only use of this cache to be through the LRUCacheInstance
        /// following the sigleton pattern.
        /// </summary>
        /// <param name="cacheCapacity"></param>
        /// <exception cref="ArgumentException"></exception>
        private LRUCache(int chosenCacheCapacity=100)
        {
            // Don't allow a negative or 0 capacity
            if (chosenCacheCapacity <= 0)
                throw new ArgumentException("capacity must be greater than 0.");
            cacheCapacity = chosenCacheCapacity;
            cacheDictionary = new Dictionary<object, CacheNode>(cacheCapacity);
            cacheDoublyLinkedList = new DoublyLinkedList();
        }

        /// <summary>
        /// Takes a key value pair and adds it to the cache if it's not already present.
        /// 
        /// If already present the value is moved to the head of the underlying linked list as the
        /// most frequently used item.
        /// 
        /// If adding to the cache brings the cache past it's capacity, the least frequently
        /// used cache node is evicted from the cache.
        /// 
        /// null calues are not valud for cache keys or values.
        /// </summary>
        /// <param name="cacheNodeKey"> Unique key to be used in cache data storage. </param>
        /// <param name="cacheNodeValue"> Value to store in cache. </param>
        /// <exception cref="ArgumentNullException"> A null value was set as the cache key or value. </exception>
        public void AddOrMoveLinkedListCacheNode(object cacheNodeKey, object cacheNodeValue)
        {
            // Throw error if key or value is null
            if (cacheNodeKey == null)
                throw new ArgumentNullException(nameof(cacheNodeKey) + " is null, which cannot be used as a key in the cache.");
            if (cacheNodeValue == null)
                throw new ArgumentNullException(nameof(cacheNodeValue) + " is null, which cannot be used as a value in the cache.");

            readWriteLockObject.EnterWriteLock();
            try
            {
                // If item is in cache, move to front of cache
                if (cacheDictionary.ContainsKey(cacheNodeKey))
                {
                    CacheNode cacheNodeToMove = cacheDictionary[cacheNodeKey];
                    cacheDoublyLinkedList.MoveNodeToHeadOfList(cacheNodeToMove);
                }
                else
                {
                    // If at capacity limit remove least recently used and notify subscribers
                    if (cacheDictionary.Count >= cacheCapacity)
                    {
                        object tailNodeKey = cacheDoublyLinkedList.Tail.CacheNodeKey;
                        cacheDictionary.Remove(tailNodeKey);
                        CacheNode? evictedNode = cacheDoublyLinkedList.EvictLRUNode();

                        // Trigger event to notify subscribed consumers about cache node eviction
                        CacheNodeEvictionEventArgs eventArgs = new CacheNodeEvictionEventArgs();
                        eventArgs.cacheNodeKey = evictedNode.CacheNodeKey;
                        eventArgs.cacheNodeValue = evictedNode.CacheNodeValue;
                        eventArgs.dateTimeEvicted = DateTime.Now;
                        OnCacheNodeEviction(eventArgs);
                    }
                    // Add new node to cache
                    CacheNode newCacheNode = new CacheNode(cacheNodeKey, cacheNodeValue);
                    cacheDictionary.Add(cacheNodeKey, newCacheNode);
                    cacheDoublyLinkedList.AddAsHead(newCacheNode);
                }
            }
            finally
            {
                readWriteLockObject.ExitWriteLock();
            }
        }

        /// <summary>
        /// Get value from cache given unique key if key is in dictionary and move
        /// cache node to the head of the cache if it's not already there.
        /// 
        /// If key is null or not in the dictinary, throw relevant excetion.
        /// </summary>
        /// <param name="cacheNodeKey"> Unique key linked to potential cache value to use for search. </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> Key given is null which is not valid in this implementation of a cache. </exception>
        /// <exception cref="KeyNotFoundException"> Given key was not found in the cache. </exception>
        public object GetCacheNodeValue(object cacheNodeKey)
        {
            if (cacheNodeKey == null)
                throw new ArgumentNullException(nameof(cacheNodeKey) + " is null, which cannot be used to retrieve a value from the cache.");

            // Read lock used to ensure that cacheNodeKey remains in the cacheDictionary with a non-null value
            // whilst reading.
            readWriteLockObject.EnterReadLock();
            try
            {
                // This section could likely be made slightly more performant if we perform the
                // check for the cache node twice, once inside the lock, once outside, so that
                // it remains thread safe but we can perform most cache miss Exception handling
                // without hindering other tasks running.
                if (cacheDictionary.ContainsKey(cacheNodeKey))
                {
                    CacheNode cacheNode = cacheDictionary[cacheNodeKey];
                    cacheDoublyLinkedList.MoveNodeToHeadOfList(cacheNode);
                    return cacheDictionary[cacheNodeKey].CacheNodeValue;
                }
                else
                    throw new KeyNotFoundException(nameof(cacheNodeKey) + " Not found in cache.");
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(nameof(cacheNodeKey) + " Not found in cache.");
            }
            finally
            {
                readWriteLockObject.ExitReadLock();
            }
        }

        /// <summary>
        /// Reset the cache, releasing all resources and clearing internal
        /// data structures without any notification process of which members
        /// were removed as they all will be.
        /// 
        /// Cache node eviction event is not reset to keep consumers subscribed.
        /// </summary>
        public void ResetCache()
        {
            lock (lockInstanceObject)
            {
                // Release resources held by the cache
                // because ReaderWriterLockSlim IDisposable, it suggests clean-up
                // is advised to avoid issues like dangling memory.
                readWriteLockObject.Dispose();

                // Clear internal Data structures
                // This clears the dictionary and LinkedList
                // The implementation of how is dealt with by the garbage collector
                // Internally, the references to the objects are removed and the
                // garbage collector will collect objects without references
                // pointing to them.
                // The LinkedList nodes however have circuulr references to
                // eachother still, this can be handled by things like generational
                // garbage collectors, to ensure these references are removed safely
                // I've repeatedly called the EvictLRUNode method, but batch removal
                // would have been used instead if I had more time.
                // I would have also tested the memory management of the Dictionary.Clear()
                while (cacheDoublyLinkedList.Head != null)
                {
                    cacheDoublyLinkedList.EvictLRUNode();
                }
                cacheDictionary.Clear();

                cacheCapacity = 100;
                readWriteLockObject = new ReaderWriterLockSlim();
            }
        }
    }
}
