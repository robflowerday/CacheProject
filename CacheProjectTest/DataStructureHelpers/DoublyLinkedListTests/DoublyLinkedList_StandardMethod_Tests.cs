﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject.DataStructureHelpers;

namespace CacheProjectTest.DataStructureHelpers.DoublyLinkedListTests
{
    [TestFixture]
    public class DoublyLinkedList_Simple_Method_Tests
    {
        [Test]
        public void AppropriateConstructor_int_string_success()
        {
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();

            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(null));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(null));
            });
        }

        [Test]
        public void AppropriateConstructor_bool_double_success()
        {
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();

            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(null));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(null));
            });
        }

        [Test]
        public void AddAsHead_empty_list()
        {
            // Arrange
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();
            CacheNode newCacheNode = new CacheNode("One", 1);

            // Act
            doublyLinkedList.AddAsHead(newCacheNode);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode));
            });
        }

        [Test]
        public void AddAsHead_three_items_list()
        {
            // Arrange
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();
            CacheNode newCacheNode1 = new CacheNode("One", 1);
            CacheNode newCacheNode2 = new CacheNode("One", 1);
            CacheNode newCacheNode3 = new CacheNode("One", 1);

            // Act
            doublyLinkedList.AddAsHead(newCacheNode1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode1));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode1));
            });

            // Act
            doublyLinkedList.AddAsHead(newCacheNode2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode2));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode1));
            });

            // Act
            doublyLinkedList.AddAsHead(newCacheNode3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode3));
                Assert.That(doublyLinkedList.Head.NextNode, Is.EqualTo(newCacheNode2));
                Assert.That(newCacheNode2.NextNode, Is.EqualTo(newCacheNode1));
                Assert.That(doublyLinkedList.Tail.PrevNode, Is.EqualTo(newCacheNode2));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode1));
            });
        }

        [Test]
        public void MoveNodeToHeadOfList_OneItem()
        {
            // Arrange
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();
            CacheNode newCacheNode = new CacheNode("One", 1);
            doublyLinkedList.AddAsHead(newCacheNode);

            // Act
            doublyLinkedList.MoveNodeToHeadOfList(newCacheNode);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode));
            });
        }

        [Test]
        public void MoveNodeToHeadOfList_ThreeItems_Move_Head()
        {
            // Arrange
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();
            CacheNode newCacheNode1 = new CacheNode("One", 1);
            CacheNode newCacheNode2 = new CacheNode("One", 1);
            CacheNode newCacheNode3 = new CacheNode("One", 1);
            doublyLinkedList.AddAsHead(newCacheNode1);
            doublyLinkedList.AddAsHead(newCacheNode2);
            doublyLinkedList.AddAsHead(newCacheNode3);

            // Act
            doublyLinkedList.MoveNodeToHeadOfList(newCacheNode3);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode3));
                Assert.That(doublyLinkedList.Head.NextNode, Is.EqualTo(newCacheNode2));
                Assert.That(doublyLinkedList.Head.PrevNode, Is.EqualTo(null));
                Assert.That(newCacheNode2.PrevNode, Is.EqualTo(newCacheNode3));
            });
        }

        [Test]
        public void MoveNodeToHeadOfList_ThreeItems_Move_Middle()
        {
            // Arrange
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();
            CacheNode newCacheNode1 = new CacheNode("One", 1);
            CacheNode newCacheNode2 = new CacheNode("One", 1);
            CacheNode newCacheNode3 = new CacheNode("One", 1);
            doublyLinkedList.AddAsHead(newCacheNode1);
            doublyLinkedList.AddAsHead(newCacheNode2);
            doublyLinkedList.AddAsHead(newCacheNode3);

            // Act
            doublyLinkedList.MoveNodeToHeadOfList(newCacheNode2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode2));
                Assert.That(doublyLinkedList.Head.NextNode, Is.EqualTo(newCacheNode3));
                Assert.That(doublyLinkedList.Head.PrevNode, Is.EqualTo(null));
                Assert.That(newCacheNode3.PrevNode, Is.EqualTo(newCacheNode2));
                Assert.That(newCacheNode3.NextNode, Is.EqualTo(newCacheNode1));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode1));
                Assert.That(doublyLinkedList.Tail.PrevNode, Is.EqualTo(newCacheNode3));
            });
        }

        [Test]
        public void MoveNodeToHeadOfList_ThreeItems_Move_Tail()
        {
            // Arrange
            DoublyLinkedList doublyLinkedList = new DoublyLinkedList();
            CacheNode newCacheNode1 = new CacheNode("One", 1);
            CacheNode newCacheNode2 = new CacheNode("One", 1);
            CacheNode newCacheNode3 = new CacheNode("One", 1);
            doublyLinkedList.AddAsHead(newCacheNode1);
            doublyLinkedList.AddAsHead(newCacheNode2);
            doublyLinkedList.AddAsHead(newCacheNode3);

            // Act
            doublyLinkedList.MoveNodeToHeadOfList(newCacheNode1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(doublyLinkedList.Head, Is.EqualTo(newCacheNode1));
                Assert.That(doublyLinkedList.Head.NextNode, Is.EqualTo(newCacheNode3));
                Assert.That(doublyLinkedList.Head.PrevNode, Is.EqualTo(null));
                Assert.That(newCacheNode3.PrevNode, Is.EqualTo(newCacheNode1));
                Assert.That(newCacheNode3.NextNode, Is.EqualTo(newCacheNode2));
                Assert.That(doublyLinkedList.Tail, Is.EqualTo(newCacheNode2));
                Assert.That(doublyLinkedList.Tail.PrevNode, Is.EqualTo(newCacheNode3));
            });
        }
    }
}

