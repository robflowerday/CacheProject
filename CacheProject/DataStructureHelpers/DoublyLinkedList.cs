﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject.DataStructureHelpers
{
    public class DoublyLinkedList
    {

        private readonly object lockObject = new object();

        public CacheNode? Head { get; set; }
        public CacheNode? Tail { get; set; }

        public DoublyLinkedList()
        {
            Head = null;
            Tail = null;
        }

        /// <summary>
        /// Add cache node to head of linked list.
        /// </summary>
        /// <param name="newCacheNode"></param>
        public void AddAsHead(CacheNode newCacheNode)
        {
            lock (lockObject)
            {
                // If linked list is empty
                if (Head == null)
                {
                    Head = newCacheNode;
                    Tail = newCacheNode;
                }
                else
                {
                    Head.PrevNode = newCacheNode;
                    newCacheNode.NextNode = Head;
                    Head = newCacheNode;
                }
            }
        }

        /// <summary>
        /// Moves a node from any point in the linked list to the front.
        /// </summary>
        /// <param name="cacheNodeToMove"> Node to move to front of linked list. </param>
        public void MoveNodeToHeadOfList(CacheNode cacheNodeToMove)
        {
            lock (lockObject)
            {
                // if cache node is tail node and not head
                if (cacheNodeToMove.NextNode == null && cacheNodeToMove.PrevNode != null)
                {
                    // Maniulate current surrounding nodes
                    cacheNodeToMove.PrevNode.NextNode = null;
                    Tail = cacheNodeToMove.PrevNode;

                    // Mainpulate new surronding node
                    Head.PrevNode = cacheNodeToMove;

                    // Manipulate node to move
                    cacheNodeToMove.PrevNode = null;
                    cacheNodeToMove.NextNode = Head;
                    Head = cacheNodeToMove;
                }
                // if cache node has previous and next node
                else if (cacheNodeToMove.NextNode != null && cacheNodeToMove.PrevNode != null)
                {
                    // Maniulate current surrounding nodes
                    cacheNodeToMove.PrevNode.NextNode = cacheNodeToMove.NextNode;
                    cacheNodeToMove.NextNode.PrevNode = cacheNodeToMove.PrevNode;

                    // Mainpulate new surronding node
                    Head.PrevNode = cacheNodeToMove;

                    // Manipulate node to move
                    cacheNodeToMove.PrevNode = null;
                    cacheNodeToMove.NextNode = Head;
                    Head = cacheNodeToMove;
                }
                // Else node to move is already the head node
            }
        }

        /// <summary>
        /// Remove and return the tail of the linked list if it exists, managing the integrity
        /// of the doubly linked list structure.
        /// </summary>
        /// <returns> Tail of the linked list, which may be null (but shouldn't in our use case). </returns>
        public CacheNode? EvictLRUNode()
        {
            lock (lockObject)
            {
                // Non-empty linked list
                if (Tail != null)
                {
                    CacheNode? evictedNode = Tail;

                    // Head of linked list is not also the tail
                    if (Tail.PrevNode != null)
                    {
                        Tail.PrevNode.NextNode = null;
                        Tail = Tail.PrevNode;
                        return evictedNode;
                    }
                    // Head of linked list is also the tail
                    Tail = null;
                    Head = null;

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
