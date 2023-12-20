using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject
{
    public class DoublyLinkedList<TCacheNodeKey, TCacheNodeValue>
    {

        private readonly object lockObject = new object();

        public CacheNode<TCacheNodeKey, TCacheNodeValue>? Head { get; set; }
        public CacheNode<TCacheNodeKey, TCacheNodeValue>? Tail { get; set; }

        public DoublyLinkedList()
        {
            this.Head = null;
            this.Tail = null;
        }

        /// <summary>
        /// Add cache node to head of linked list.
        /// </summary>
        /// <param name="newCacheNode"></param>
        public void AddAsHead(CacheNode<TCacheNodeKey, TCacheNodeValue> newCacheNode)
        {
            lock (lockObject)
            {
                // If linked list is empty
                if (this.Head == null)
                {
                    this.Head = newCacheNode;
                    this.Tail = newCacheNode;
                }
                else
                {
                    this.Head.PrevNode = newCacheNode;
                    newCacheNode.NextNode = this.Head;
                    this.Head = newCacheNode;
                }
            }
        }

        public void MoveNodeToHeadOfList(CacheNode<TCacheNodeKey, TCacheNodeValue>  cacheNodeToMove)
        {
            lock (lockObject)
            {
                // if cache node is tail node and not head
                if (cacheNodeToMove.NextNode == null && cacheNodeToMove.PrevNode != null)
                {
                    // Maniulate current surrounding nodes
                    cacheNodeToMove.PrevNode.NextNode = null;
                    this.Tail = cacheNodeToMove.PrevNode;

                    // Mainpulate new surronding node
                    this.Head.PrevNode = cacheNodeToMove;

                    // Manipulate node to move
                    cacheNodeToMove.PrevNode = null;
                    cacheNodeToMove.NextNode = this.Head;
                    this.Head = cacheNodeToMove;
                }
                // if cache node has previous and next node
                else if (cacheNodeToMove.NextNode != null && cacheNodeToMove.PrevNode != null)
                {
                    // Maniulate current surrounding nodes
                    cacheNodeToMove.PrevNode.NextNode = cacheNodeToMove.NextNode;
                    cacheNodeToMove.NextNode.PrevNode = cacheNodeToMove.PrevNode;

                    // Mainpulate new surronding node
                    this.Head.PrevNode = cacheNodeToMove;

                    // Manipulate node to move
                    cacheNodeToMove.PrevNode = null;
                    cacheNodeToMove.NextNode = this.Head;
                    this.Head = cacheNodeToMove;
                }
                // Else node to move is already the head node
            }
        }

        /// <summary>
        /// Remove and return the tail of the linked list if it exists, managing the integrity
        /// of the doubly linked list structure.
        /// </summary>
        /// <returns> Tail of the linked list, which may be null (but shouldn't in our use case). </returns>
        public CacheNode<TCacheNodeKey, TCacheNodeValue>? EvictLRUNode()
        {
            lock (lockObject)
            {
                // Non-empty linked list
                if (this.Tail != null)
                {
                    CacheNode<TCacheNodeKey, TCacheNodeValue>? evictedNode = this.Tail;

                    // Head of linked list is not also the tail
                    if (this.Tail.PrevNode != null)
                    {
                        this.Tail.PrevNode.NextNode = null;
                        this.Tail = this.Tail.PrevNode;
                        return evictedNode;
                    }
                    // Head of linked list is also the tail
                    this.Tail = null;
                    this.Head = null;

                    // set references to other nodes to nulll to avoid
                    // potential memory leaks (though I believe this would
                    // be handled by C#s garbage collector using something
                    // like a generational garbage collector.)
                    evictedNode.PrevNode = null;
                    evictedNode.NextNode = null;

                    return evictedNode;
                }
                // Empty linked list
                return null;
            }
        }
    }
}
