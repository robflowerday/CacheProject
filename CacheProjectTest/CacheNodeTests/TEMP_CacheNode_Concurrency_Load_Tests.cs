using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject;

namespace CacheProjectTest.CacheNode.Tests
{
    [TestFixture]
    public class CacheNode_Concurrency_Load_Tests
    {
        [Test]
        public void ConcurrentReads_NoException()
        {
            const int numThreads = 10;
            CacheNode<int, string> cacheNode = new CacheNode<int, string>(1, "value");

            Parallel.For(0, numThreads, _ =>
            {
                var prevNode = cacheNode.PrevNode;
                var nextNode = cacheNode.NextNode;
            });
        }

        [Test]
        public void ConcurrentWrites_NoException_ChangesValue()
        {
            int numThreads = 10;
            CacheNode<int, string> cacheNode = new CacheNode<int, string>(1, "value");

            Assert.That(cacheNode.NextNode, Is.Null);

            Parallel.For(0, numThreads, _ =>
            {
                CacheNode<int, string> newNode = new CacheNode<int, string>(2, "new value");
                cacheNode.PrevNode = newNode;
                cacheNode.NextNode = newNode;
            });

            Assert.That(cacheNode.NextNode.Value, Is.EqualTo("new value"));
        }

        [Test]
        public void ConcurrentReadsAndWrites_NoException_ChangesValue()
        {
            int numThreads = 10;
            CacheNode<int, string> cacheNode = new CacheNode<int, string>(1, "value");

            Assert.That(cacheNode.NextNode, Is.Null);

            Parallel.For(0, numThreads, _ =>
            {
            
            });

            Parallel.For(0, numThreads, _ =>
            {
                CacheNode<int, string> newNode = new CacheNode<int, string>(2, "new value");

                // Read
                var prevNode = cacheNode.PrevNode;
                // Write
                cacheNode.NextNode = newNode;
            });

            Assert.That(cacheNode.NextNode.Value, Is.EqualTo("new value"));
        }
    }
}

