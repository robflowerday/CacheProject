using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject.DataStructureHelpers
{
    /// <summary>
    /// Represents a node within a doubly linked list, designed for
    /// storing key-value pairs in an LRU (Least Recently Used)
    /// cache.
    /// 
    /// Key and Value are of generic type and are immutable to aid
    /// thread safety and data integrity.
    /// </summary>
    /// <typeparam name="TCacheNodeKey">The type of the key.</typeparam>
    /// <typeparam name="TCacheNodeValue">The type of the value.</typeparam>
    public class CacheNode<TCacheNodeKey, TCacheNodeValue>
    {
        // PrevNode and NextNode should be mutable, but also thread
        // safe. Using reader-writer locks allows for simultaneous
        // reads but only 1 write at a time, saving time but
        // maintaining data integrity.
        private readonly ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();

        // Key and Value are of generic type and are immutable
        // aiding data integrity, particularly when thread-safety
        // is important.
        public TCacheNodeKey CacheNodeKey { get; }
        public TCacheNodeValue CacheNodeValue { get; }

        // PrevNode and NextNode are initialised to null indicating
        // head and tail of the doubly linked list.
        private CacheNode<TCacheNodeKey, TCacheNodeValue>? prevNode;
        private CacheNode<TCacheNodeKey, TCacheNodeValue>? nextNode;

        public CacheNode(TCacheNodeKey pCacheNodeKey, TCacheNodeValue pCacheNodeValue)
        {
            CacheNodeKey = pCacheNodeKey;
            CacheNodeValue = pCacheNodeValue;
            PrevNode = null;
            NextNode = null;
        }

        // Allow getting and setting of the Previous and Next nodes
        // whilst ensuring thread-safety. If a write attempt is made
        // whilst the object is being written to or read, the attempting
        // thread will wait until the lock is Exited.
        public CacheNode<TCacheNodeKey, TCacheNodeValue>? PrevNode
        {
            get
            {
                try
                {
                    lockObject.EnterReadLock();
                    return prevNode;
                }
                finally
                {
                    lockObject.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    lockObject.EnterWriteLock();
                    prevNode = value;
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
            }
        }

        public CacheNode<TCacheNodeKey, TCacheNodeValue>? NextNode
        {
            get
            {
                try
                {
                    lockObject.EnterReadLock();
                    return nextNode;
                }
                finally
                {
                    lockObject.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    lockObject.EnterWriteLock();
                    nextNode = value;
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
            }
        }
    }

}


