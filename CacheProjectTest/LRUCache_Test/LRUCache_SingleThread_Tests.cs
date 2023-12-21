using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject;
using NuGet.Frameworks;
using System.Xml.Serialization;

namespace CacheProjectTest.LRUCacheTests
{
    [TestFixture]
    public class LRUCache_SingleThread_Tests
    {
        [Test]
        public void LRUCache_NegativeCapacity_ThrowsError()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            int capacity = -1;

            // Act + Assert
            Assert.Throws<ArgumentException>(() => lruCacheInstance.SetCacheCapacity(capacity));
        }

        [Test]
        public void LRUCache_ZeroCapacity_ThrowsError()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            int capacity = 0;

            // Act + Assert
            Assert.Throws<ArgumentException>(() => lruCacheInstance.SetCacheCapacity(capacity));
        }

        /// <summary>
        /// I do not believe parallel tests themselves when run are
        /// thread safe, to get around this, rather than using a lock
        /// I've used a key that has not been used elsewhere. Success
        /// still isn't gaurenteed if large numbers of successive node
        /// removals occur at the wrong time.
        /// </summary>
        [Test]
        public void LRUCache_GetCacheNodeValue_Move_Exists()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key1", "val1");
            lruCacheInstance.AddOrMoveLinkedListCacheNode("getcachenodetestkey", "getcachenodetestvalue");
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key3", "val3");

            // Act
            object cacheNodeValue = lruCacheInstance.GetCacheNodeValue("getcachenodetestkey");

            // Assert
            Assert.That(cacheNodeValue, Is.EqualTo("getcachenodetestvalue"));

            // Act (ensure that only 1 of the previous values is left in the cache)
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key4", "val4");
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key5", "val5");
            object cacheNodeValueAgain = lruCacheInstance.GetCacheNodeValue("getcachenodetestkey");

            // Assert
            Assert.That(cacheNodeValueAgain, Is.EqualTo("getcachenodetestvalue"));
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_DoesntExist()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key1", true);

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => lruCacheInstance.GetCacheNodeValue("Key That is never used"));
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_EmptyCache()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => lruCacheInstance.GetCacheNodeValue("key"));
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_NullKey()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCacheInstance.GetCacheNodeValue(null));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_empty()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key", true);

            // Assert
            Assert.That(lruCacheInstance.GetCacheNodeValue("key"), Is.EqualTo(true));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AddNullKey()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCacheInstance.AddOrMoveLinkedListCacheNode(null, false));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AddNullValue()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCacheInstance.AddOrMoveLinkedListCacheNode("key", null));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AddNullKeyAndValue()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCacheInstance.AddOrMoveLinkedListCacheNode(null, null));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_nonempty()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key1", true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key2", false);

            // Act
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key1", true);

            // Assert
            Assert.That(lruCacheInstance.GetCacheNodeValue("key1"), Is.EqualTo(true));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AtCapacity()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key1", true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key2", false);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key3", true);

            // Act
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key4", false);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(lruCacheInstance.GetCacheNodeValue("key2"), Is.False);
                Assert.Throws<KeyNotFoundException>(() => lruCacheInstance.GetCacheNodeValue("key1"));
            });
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_MoveExisitngNodeToFront()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key1", true);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key2", false);
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key3", true);

            // Act
            lruCacheInstance.AddOrMoveLinkedListCacheNode("key2", false);

            // Assert
            // Verify all nodes are still in cache
            Assert.Multiple(() =>
            {
                Assert.That(lruCacheInstance.GetCacheNodeValue("key1"), Is.True);
                Assert.That(lruCacheInstance.GetCacheNodeValue("key2"), Is.False);
                Assert.That(lruCacheInstance.GetCacheNodeValue("key3"), Is.True);
            });
        }
    }

}

