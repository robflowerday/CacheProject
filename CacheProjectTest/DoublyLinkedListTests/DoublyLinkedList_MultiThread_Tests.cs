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
            // Tasks run in parallel, tests that the lock ensures no run condition errors
            Parallel.For(0, 100, i =>
            {
                Task task = Task.Run(() => doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i)));
                tasks.Add(task);
            });

            Task.WaitAll(tasks.ToArray(), millisecondsTimeout: 30000); // Allow 30 seconds before failing

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
            // Tasks run in parallel, tests that the lock ensures no run condition errors
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
            // Tasks run in parallel, tests that the lock ensures no run condition errors
            Parallel.For(0, 50, i =>
            {
                Task task = Task.Run(() => doublyLinkedList.EvictLRUNode());
                if (task != null)
                    tasks.Add(task);
            });

            // Can't use WaitAll as we expect some null values to be possible
            Task.WaitAll(tasks.ToArray(), millisecondsTimeout: 30000); // Alow 30 seconds before timeout

            // Assert
            Assert.That(NonEmptyLinkedListLength(doublyLinkedList), Is.EqualTo(50));
        }

        [Test]
        public void MultiThread_MixedMethods_ExceptionNotThrown()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 500; i++)
            {
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
            }

            CacheNode<string, int> cacheNode = doublyLinkedList.Tail;

            // Act
            // Run the method MoveNodeToHeadOfList 100 times as a task
            // Tasks run in parallel, tests that the lock ensures no run condition errors
            Parallel.For(0, 300, i =>
            {
                tasks.Add(Task.Run(() => doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i))));
                // not thread safe because Tail can be changed after setting in and prior to (and is regularly changed by EvictLRUNode)
                // test case that will pass almost every time is to use a large list (one larger that the number of nodes being removed
                // in total) and doublyLinkedList.Head.nextNode.
                tasks.Add(Task.Run(() => doublyLinkedList.MoveNodeToHeadOfList(doublyLinkedList.Head.NextNode)));
                tasks.Add(Task.Run(() => doublyLinkedList.EvictLRUNode()));
            });

            Task.WaitAll(tasks.ToArray(), millisecondsTimeout: 30000); // Allow 30 second timeout

            // Assert
            // Ensure no error is thrown
        }
    }
}

