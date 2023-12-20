using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject.DataStructureHelpers;

namespace CacheProjectTest.DataStructureHelpers.CacheNodeTests
{
    [TestFixture]
    public class CacheNode_Creation_Tests
    {
        [Test]
        public void CreateCacheNodeStringValue()
        {
            // Arrange
            int key = 1;
            string value = "Value";

            // Act
            CacheNode cacheNode = new CacheNode(key, value);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(key, Is.EqualTo(cacheNode.CacheNodeKey));
                Assert.That(value, Is.EqualTo(cacheNode.CacheNodeValue));
                Assert.That(cacheNode.PrevNode, Is.Null);
                Assert.That(cacheNode.NextNode, Is.Null);
            });
        }

        [Test]
        public void CreateCacheNodeIntValue()
        {
            // Arrange
            int key = 1;
            int value = 2;

            // Act
            CacheNode cacheNode = new CacheNode(key, value);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(key, Is.EqualTo(cacheNode.CacheNodeKey));
                Assert.That(value, Is.EqualTo(cacheNode.CacheNodeValue));
                Assert.That(cacheNode.PrevNode, Is.Null);
                Assert.That(cacheNode.NextNode, Is.Null);
            });
        }

        [Test]
        public void CreateCacheNodeBoolValue()
        {
            // Arrange
            int key = 1;
            bool value = true;

            // Act
            CacheNode cacheNode = new CacheNode(key, value);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(key, Is.EqualTo(cacheNode.CacheNodeKey));
                Assert.That(value, Is.EqualTo(cacheNode.CacheNodeValue));
                Assert.That(cacheNode.PrevNode, Is.Null);
                Assert.That(cacheNode.NextNode, Is.Null);
            });
        }

        /// <summary>
        /// Helper class for testing purposes.
        /// </summary>
        private class TestClass
        {
            public string Data { get; }

            public TestClass(string Data)
            {
                this.Data = Data;
            }
        }

        [Test]
        public void CreateCacheNodeClassValue()
        {
            // Arrange
            int key = 1;
            TestClass value = new TestClass("TestValue");

            // Act
            CacheNode cacheNode = new CacheNode(key, value);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(key, Is.EqualTo(cacheNode.CacheNodeKey));
                Assert.That(value, Is.EqualTo(cacheNode.CacheNodeValue));
                Assert.That(cacheNode.PrevNode, Is.Null);
                Assert.That(cacheNode.NextNode, Is.Null);
            });
        }
    }
}

