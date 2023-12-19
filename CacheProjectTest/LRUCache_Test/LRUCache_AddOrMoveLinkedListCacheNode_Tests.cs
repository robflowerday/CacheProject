using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject;
using NuGet.Frameworks;

namespace CacheProjectTest.LRUCacheTests
{
    [TestFixture]
    public class LRUCache_AddOrMoveLinkedListCacheNode_Tests
    {
        [Test]
        public void LRUCache_GetCacheNodeValue_Exists()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);
            lruCache.AddOrMoveLinkedListCacheNode("key", true);

            // Act
            bool cacheNodeValue = lruCache.GetCacheNodeValue("key");

            // Assert
            Assert.That(cacheNodeValue, Is.True);
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_DoesntExist()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);
            lruCache.AddOrMoveLinkedListCacheNode("key", true);

            // Act
            bool cacheNodeValue = lruCache.GetCacheNodeValue("key1");

            // Assert
            Assert.That(cacheNodeValue, Is.EqualTo(null));
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_EmptyCache()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);

            // Act
            bool cacheNodeValue = lruCache.GetCacheNodeValue("key");

            // Assert
            Assert.That(cacheNodeValue, Is.EqualTo(null));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_empty()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);

            // Act
            lruCache.AddOrMoveLinkedListCacheNode("key", true);

            // Assert
            Assert.That(lruCache.GetCacheNodeValue("key"), Is.EqualTo(true));
        }







        [Test]
        public void LRUCache_Add_EmptyCache()
        {
            //Arrange
            LRUCache<int, string> cache = new LRUCache<int, string>(3);

            // Act
            cache.AddOrMoveLinkedListCacheNode(1, "value");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cache.CacheDoublyLinkedList.Head.NextNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Head.PrevNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Head.CacheNodeValue, Is.EqualTo("value"));
                Assert.That(cache.CacheDoublyLinkedList.Head.CacheNodeKey, Is.EqualTo(1));
                Assert.That(cache.CacheDoublyLinkedList.Tail.NextNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Tail.PrevNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Tail.CacheNodeValue, Is.EqualTo("value"));
                Assert.That(cache.CacheDoublyLinkedList.Tail.CacheNodeKey, Is.EqualTo(1));
            });
        }

        [Test]
        public void LRUCache_Add_Move_SingleItem()
        {
            //Arrange
            LRUCache<int, string> cache = new LRUCache<int, string>(3);

            // Act
            cache.AddOrMoveLinkedListCacheNode(1, "value");
            cache.AddOrMoveLinkedListCacheNode(1, "value");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cache.CacheDoublyLinkedList.Head.NextNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Head.PrevNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Head.CacheNodeValue, Is.EqualTo("value"));
                Assert.That(cache.CacheDoublyLinkedList.Head.CacheNodeKey, Is.EqualTo(1));
                Assert.That(cache.CacheDoublyLinkedList.Tail.NextNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Tail.PrevNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Tail.CacheNodeValue, Is.EqualTo("value"));
                Assert.That(cache.CacheDoublyLinkedList.Tail.CacheNodeKey, Is.EqualTo(1));
            });
        }

        [Test]
        public void LRUCache_Add_Add_Move()
        {
            //Arrange
            LRUCache<int, string> cache = new LRUCache<int, string>(3);

            // Act
            cache.AddOrMoveLinkedListCacheNode(1, "first value");
            cache.AddOrMoveLinkedListCacheNode(2, "second value");
            cache.AddOrMoveLinkedListCacheNode(1, "first value");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cache.CacheDoublyLinkedList.Head.NextNode, Is.EqualTo(cache.CacheDoublyLinkedList.Tail));
                Assert.That(cache.CacheDoublyLinkedList.Head.PrevNode, IsNot.Null);
                Assert.That(cache.CacheDoublyLinkedList.Head.CacheNodeValue, Is.EqualTo("first value"));
                Assert.That(cache.CacheDoublyLinkedList.Head.CacheNodeKey, Is.EqualTo(1));
                Assert.That(cache.CacheDoublyLinkedList.Tail.NextNode, Is.Null);
                Assert.That(cache.CacheDoublyLinkedList.Tail.PrevNode, Is.EqualTo(cache.CacheDoublyLinkedList.Head));
                Assert.That(cache.CacheDoublyLinkedList.Tail.CacheNodeValue, Is.EqualTo("second value"));
                Assert.That(cache.CacheDoublyLinkedList.Tail.CacheNodeKey, Is.EqualTo(2));
            });
        }
    }

}
