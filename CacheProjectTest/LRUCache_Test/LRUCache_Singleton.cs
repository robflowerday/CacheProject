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
    public class LRUCache_Singleton_Tests
    {

        /// <summary>
        /// Create 2 instances of LRUCache and check that they reference the
        /// same object.
        /// </summary>
        [Test]
        public void LRUCache_isSingleton()
        {
            // Arrange + Act
            LRUCache<string?, bool?> lruCacheInstance1 = LRUCache<string?, bool?>.LRUCacheInstance;
            LRUCache<string?, bool?> lruCacheInstance2 = LRUCache<string?, bool?>.LRUCacheInstance;

            // Assert
            Assert.AreSame(lruCacheInstance1, lruCacheInstance2, "LRUCache instances should point to the same reference meaning they are the same object and follow the singleton design pattern.");
        }
    }

}

