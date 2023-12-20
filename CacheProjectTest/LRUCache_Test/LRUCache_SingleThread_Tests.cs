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
            // Nothing to arrange

            // Act + Assert
            Assert.Throws<ArgumentException>(() => new LRUCache<string, int>(-1));
        }

        [Test]
        public void LRUCache_ZeroCapacity_ThrowsError()
        {
            // Arrange
            // Nothing to arrange

            // Act + Assert
            Assert.Throws<ArgumentException>(() => new LRUCache<string, int>(0));
        }

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
            lruCache.AddOrMoveLinkedListCacheNode("key1", true);

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => lruCache.GetCacheNodeValue("key2"));
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_EmptyCache()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => lruCache.GetCacheNodeValue("key"));
        }

        [Test]
        public void LRUCache_GetCacheNodeValue_NullKey()
        {
            // Arrange
            LRUCache<string?, bool> lruCache = new LRUCache<string?, bool>(3);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCache.GetCacheNodeValue(null));
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
        public void LRUCache_AddOrMoveLinkedListCacheNode_AddNullKey()
        {
            // Arrange
            LRUCache<string?, bool?> lruCache = new LRUCache<string?, bool?>(3);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCache.AddOrMoveLinkedListCacheNode(null, false));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AddNullValue()
        {
            // Arrange
            LRUCache<string?, bool?> lruCache = new LRUCache<string?, bool?>(3);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCache.AddOrMoveLinkedListCacheNode("key", null));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AddNullKeyAndValue()
        {
            // Arrange
            LRUCache<string?, bool?> lruCache = new LRUCache<string?, bool?>(3);

            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => lruCache.AddOrMoveLinkedListCacheNode(null, null));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_nonempty()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);
            lruCache.AddOrMoveLinkedListCacheNode("key1", true);
            lruCache.AddOrMoveLinkedListCacheNode("key2", false);

            // Act
            lruCache.AddOrMoveLinkedListCacheNode("key1", true);

            // Assert
            Assert.That(lruCache.GetCacheNodeValue("key1"), Is.EqualTo(true));
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_AtCapacity()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);
            lruCache.AddOrMoveLinkedListCacheNode("key1", true);
            lruCache.AddOrMoveLinkedListCacheNode("key2", false);
            lruCache.AddOrMoveLinkedListCacheNode("key3", true);

            // Act
            lruCache.AddOrMoveLinkedListCacheNode("key4", false);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(lruCache.GetCacheNodeValue("key2"), Is.False);
                Assert.Throws<KeyNotFoundException>(() => lruCache.GetCacheNodeValue("key1"));
            });
        }

        [Test]
        public void LRUCache_AddOrMoveLinkedListCacheNode_MoveExisitngNodeToFront()
        {
            // Arrange
            LRUCache<string, bool> lruCache = new LRUCache<string, bool>(3);
            lruCache.AddOrMoveLinkedListCacheNode("key1", true);
            lruCache.AddOrMoveLinkedListCacheNode("key2", false);
            lruCache.AddOrMoveLinkedListCacheNode("key3", true);

            // Act
            lruCache.AddOrMoveLinkedListCacheNode("key2", false);

            // Assert
            // Verify all nodes are still in cache
            Assert.Multiple(() =>
            {
                Assert.That(lruCache.GetCacheNodeValue("key1"), Is.True);
                Assert.That(lruCache.GetCacheNodeValue("key2"), Is.False);
                Assert.That(lruCache.GetCacheNodeValue("key3"), Is.True);
            });
        }
    }

}

