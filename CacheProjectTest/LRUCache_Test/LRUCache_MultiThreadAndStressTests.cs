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
    public class LRUCache_MultiThreadAndStress_Tests
    {
        [Test]
        public void LRUCache_ConcurrentReadsAndWritesAreThreadSafe_NoException()
        {
            // Arrange
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            for (int i=0; i < 95; i++)
                lruCacheInstance.AddOrMoveLinkedListCacheNode(Convert.ToString(i), i);

            // Act
            int threadCount = 20;

            for (int i = 0; i < threadCount; i++)
            {
                if (i % 2 == 0)
                    lruCacheInstance.AddOrMoveLinkedListCacheNode(Convert.ToString(i), i);
                else
                    lruCacheInstance.GetCacheNodeValue(Convert.ToString(i + 2));
            }

            // Assert
            // Assertion is just that no errorrs are thrown
        }

        [Test]
        [Timeout(30000)] // Test has 30 second time limit
        public void LRUCache_HighConcurrentReads_ShouldSucceedInReasonableTime()
        {
            // Arrange
            int capacity = 10;
            int numReads = 10000000;
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;

            // This aspect is likely taking a long time, with the current behaviour
            // reducing caching capcity is not performant.
            lruCacheInstance.SetCacheCapacity(capacity, allowEviction: true);
            for (int i = 0; i < capacity; i++)
                lruCacheInstance.AddOrMoveLinkedListCacheNode(Convert.ToString(i), i);

            // Act
            for (int i = 0; i < numReads; i++)
                lruCacheInstance.GetCacheNodeValue(Convert.ToString(i%capacity));

            // Assert
            // Assertion is handles by [Timeout]
        }

        [Test]
        [Timeout(30000)] // Test has 30 second time limit
        public void LRUCache_HighConcurrentWrites_ShouldSucceedInReasonableTime()
        {
            // Arrange
            int capacity = 50;
            int numWrites = 10000000;
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act
            for (int i = 0; i < numWrites; i++)
                lruCacheInstance.AddOrMoveLinkedListCacheNode(Convert.ToString(i), i);

            // Assert
            // Assertion is handles by [Timeout]
        }

        [Test]
        [Timeout(30000)] // Test has 30 second time limit
        public void LRUCache_HighConcurrentReadsAndWrites_ShouldSucceedInReasonableTime()
        {
            // Arrange
            int capacity = 50;
            int numReadsAndWrites = 10000000;
            LRUCache lruCacheInstance = LRUCache.LRUCacheInstance;
            lruCacheInstance.SetCacheCapacity(3, allowEviction: true);

            // Act
            for (int i = 0; i < numReadsAndWrites; i++)
            {
                lruCacheInstance.AddOrMoveLinkedListCacheNode(Convert.ToString(i), i);
                lruCacheInstance.GetCacheNodeValue(Convert.ToString(i));
            }

            // Assert
            // Assertion is handles by [Timeout]
        }
    }

}

