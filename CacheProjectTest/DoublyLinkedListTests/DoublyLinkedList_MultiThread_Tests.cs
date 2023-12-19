using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject;
using System.Globalization;

namespace CacheProjectTest.DoublyLinkedListTests
{
    [TestFixture]
    public class DoublyLinkedList_MultiThread_Tests
    {
        private int NonEmptyLinkedListLength(DoublyLinkedList<string, int> doublyLinkedList)
        {
            int length = 0;
            CacheNode<string, int> currentNode = doublyLinkedList.Head;
            
            while (currentNode != null)
            {
                currentNode = currentNode.NextNode;
                length++;
            }
            return length;
        }

        [Test]
        public void MultiThread_AddAsHead_SafeAndAccurate()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();
            List<Task> tasks = new List<Task>();

            // Act
            // Run the AddAsHead method 100 times as a task
            Parallel.For(0, 100, i =>
            {
                CacheNode<string, int> newCacheNode = new CacheNode<string, int>(Convert.ToString(i), i);
                tasks.Add(Task.Run(() => doublyLinkedList.AddAsHead(newCacheNode)));
            });

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.That(NonEmptyLinkedListLength(doublyLinkedList), Is.EqualTo(100));
        }

        [Test]
        public void MultiThread_MoveNodeToHeadOfList_ExceptionNotThrown()
        {
            // Difficult to test this tasks working in a multi thread environment
            // as the order of completion is not gaurenteed. Using methods like
            // ContinueWith would achieve this, but would also be what is determining
            // the order of the threading and therefore is not appropriate for testing.

            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
            }

            CacheNode<string, int> cacheNode = doublyLinkedList.Tail;

            // Act
            // Run the method MoveNodeToHeadOfList 100 times as a task
            Parallel.For(0, 100, i =>
            {
                if (cacheNode.NextNode != null)
                {
                    tasks.Add(Task.Run(() => doublyLinkedList.MoveNodeToHeadOfList(cacheNode.NextNode)));
                    cacheNode = doublyLinkedList.Tail;
                }
            });

            Task.WaitAll(tasks.ToArray());

            // Assert
            // Ensure no error is thrown
        }

        [Test]
        public void MultiThread_EvictLRUNode_AppropriateState()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
            }

            // Act
            // Run the method MoveNodeToHeadOfList 100 times as a task
            Parallel.For(0, 50, i =>
            {
                tasks.Add(Task.Run(() => doublyLinkedList.EvictLRUNode()));
            });

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.That(NonEmptyLinkedListLength(doublyLinkedList), Is.EqualTo(50));
        }

        [Test]
        public void MultiThread_MixedMethods_ExceptionNotThrown()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
            }

            CacheNode<string, int> cacheNode = doublyLinkedList.Tail;

            // Act
            // Run the method MoveNodeToHeadOfList 100 times as a task
            Parallel.For(0, 1000, i =>
            {
                CacheNode<string, int> cacheNodeToMove = doublyLinkedList.Tail;
                CacheNode<string, int> cacheNodeToAdd = new CacheNode<string, int>(Convert.ToString(i), i);
                tasks.Add(Task.Run(() => doublyLinkedList.AddAsHead(cacheNodeToAdd)));
                tasks.Add(Task.Run(() => doublyLinkedList.MoveNodeToHeadOfList(cacheNodeToMove)));
                tasks.Add(Task.Run(() => doublyLinkedList.EvictLRUNode()));
            });

            Task.WaitAll(tasks.ToArray());

            // Assert
            // Ensure no error is thrown
        }
    }
}

