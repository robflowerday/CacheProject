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
        // Event for notifying consumers when items are evicted
        // Uses custom event handler
        public event EventHandler

        // Instance of cache to enable use of LRUCache in a singleton pattern
        // The static nature of the variable ensures only one is instantiated
        // rather than allowing multiple instances of the class.
        private static LRUCache<TCacheNodeKey, TCacheNodeValue>? lruCacheInstance;

        // Lock object used to ensure thread safety that distinguishes between reading
        // and writing for better performance when reading can be allowed
        private ReaderWriterLockSlim readWriteLockObject = new ReaderWriterLockSlim();
        // lock object for creating a single instance when differenciating between
        // reading and writing is not neccessary.
        private static readonly object lockInstanceObject = new object();

        // All data structures here are static as there is only one intended use of this class.
        // Arbitrary default capacity to 100
        // Currently set immedeately after initilisation, want to change setting to be during init
        // ArrayWithOffset choice.
        private static int cacheCapacity;
        private static Dictionary<TCacheNodeKey, CacheNode<TCacheNodeKey, TCacheNodeValue>> cacheDictionary;
        private static DoublyLinkedList<TCacheNodeKey, TCacheNodeValue> cacheDoublyLinkedList;

        // Getting of capacity is single atommic operation and so read lock is unneccessary
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

                // Evict nodes if neccessary (this can be optised by creating EvictManyLRU method
                // in DoublyLinkedList.cs which does not change pointers to other LinkedList nodes
                // until all but the last eviction is complete. This would require thinking about
                // how the nodes that still have references to them are removed from the heap -
                // I believe this would be managed by the generational garbage collector.)
                while (cacheDictionary.Count > cacheCapacity)
                {
                    // Determine key of tail node
                    TCacheNodeKey tailNodeKey = cacheDoublyLinkedList.Tail.CacheNodeKey;

                    // Remove Dictionary value with this Key
                    cacheDictionary.Remove(tailNodeKey);

                    // Remove tail node from DoublyLinkedList
                    CacheNode<TCacheNodeKey, TCacheNodeValue>? evictedNode = cacheDoublyLinkedList.EvictLRUNode();
                }
            }
            finally
            {
                readWriteLockObject.ExitWriteLock();
            }
        }

        // Set accessable Property as instance for the lruCache
        public static LRUCache<TCacheNodeKey, TCacheNodeValue> LRUCacheInstance
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
                        // if statement (null coallease assignment) to ensure only one instance is created if multiple
                        // threads enter previous if statement before new instance is created
                        lruCacheInstance ??= new LRUCache<TCacheNodeKey, TCacheNodeValue>(chosenCacheCapacity: 100);
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
            cacheDictionary = new Dictionary<TCacheNodeKey, CacheNode<TCacheNodeKey, TCacheNodeValue>>(cacheCapacity);
            cacheDoublyLinkedList = new DoublyLinkedList<TCacheNodeKey, TCacheNodeValue>();
        }

        public void AddOrMoveLinkedListCacheNode(TCacheNodeKey pCacheNodeKey, TCacheNodeValue pCacheNodeValue)
        {
            // Throw error if key or value is null
            if (pCacheNodeKey == null)
                throw new ArgumentNullException(nameof(pCacheNodeKey) + " is null, which cannot be used as a key in the cache.");
            if (pCacheNodeValue == null)
                throw new ArgumentNullException(nameof(pCacheNodeValue) + " is null, which cannot be used as a value in the cache.");

            readWriteLockObject.EnterWriteLock();
            try
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
                    if (cacheDictionary.Count >= cacheCapacity)
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
            finally
            {
                readWriteLockObject.ExitWriteLock();
            }
        }

        public TCacheNodeValue GetCacheNodeValue(TCacheNodeKey pCacheNodeKey)
        {
            if (pCacheNodeKey == null)
                throw new ArgumentNullException(nameof(pCacheNodeKey) + " is null, which cannot be used to retrieve a value from the cache.");

            // Read lock used to ensure that pCacheNodeKey remains in the cacheDictionary with a non-null value
            // whilst reading.
            readWriteLockObject.EnterReadLock();
            try
            {
                if (cacheDictionary.ContainsKey(pCacheNodeKey))
                    return cacheDictionary[pCacheNodeKey].CacheNodeValue;
                else
                    throw new KeyNotFoundException(nameof(pCacheNodeKey) + " Not found in cache.");
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(nameof(pCacheNodeKey) + " Not found in cache.");
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

                // Reset cachCapacity to default value (100)
                cacheCapacity = 100;

                // Reinitialise read-write lock boject
                readWriteLockObject = new ReaderWriterLockSlim();
            }
        }
    }
}
