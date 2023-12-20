using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject;
using System.Globalization;
using CacheProject.DataStructureHelpers;

namespace CacheProjectTest.DataStructureHelpers.DoublyLinkedListTests
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

            // Act
            // Run the AddAsHead method 100 times as a task
            // Tasks run in parallel, tests that the lock ensures no run condition errors
            Parallel.For(0, 100, i =>
            {
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
            });

            // Assert
            Assert.That(NonEmptyLinkedListLength(doublyLinkedList), Is.EqualTo(100));
        }

        [Test]
        public void MultiThread_MoveNodeToHeadOfList_ExceptionNotThrown()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();
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
                    doublyLinkedList.MoveNodeToHeadOfList(cacheNode.NextNode);
                    cacheNode = doublyLinkedList.Tail;
                }
            });

            // Assert
            // Ensure no error is thrown
        }

        [Test]
        public void MultiThread_EvictLRUNode_AppropriateState()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();

            for (int i = 0; i < 100; i++)
            {
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
            }

            // Act
            Parallel.For(0, 50, i =>
            {
                doublyLinkedList.EvictLRUNode();
            });

            // Assert
            Assert.That(NonEmptyLinkedListLength(doublyLinkedList), Is.EqualTo(50));
        }

        [Test]
        public void MultiThread_MixedMethods_ExceptionNotThrown()
        {
            // Arrange
            DoublyLinkedList<string, int> doublyLinkedList = new DoublyLinkedList<string, int>();

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
                doublyLinkedList.AddAsHead(new CacheNode<string, int>(Convert.ToString(i), i));
                // not thread safe because Tail can be changed after setting in and prior to (and is regularly changed by EvictLRUNode)
                // test case that will pass almost every time is to use a large list (one larger that the number of nodes being removed
                // in total) and doublyLinkedList.Head.nextNode.
                doublyLinkedList.MoveNodeToHeadOfList(doublyLinkedList.Head.NextNode);
                doublyLinkedList.EvictLRUNode();
            });

            // Assert
            // Ensure no error is thrown
        }
    }
}

