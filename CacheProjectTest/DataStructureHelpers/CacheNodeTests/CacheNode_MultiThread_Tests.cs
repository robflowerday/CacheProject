﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CacheProject;
using CacheProject.DataStructureHelpers;

namespace CacheProjectTest.DataStructureHelpers.CacheNodeTests
{
    [TestFixture]
    public class CacheNode_MultiThread_Tests
    {
        [Test]
        public void MultiThreadReads_NoException()
        {
            // Arrange
            const int numThreads = 10;
            CacheNode cacheNode = new CacheNode(1, "value");

            // Act
            Parallel.For(0, numThreads, _ =>
            {
                var prevNode = cacheNode.PrevNode;
                var nextNode = cacheNode.NextNode;
            });

            // Assertion is implicit as testing for no exception
        }

        [Test]
        public void MultiThreadWrites_NoException_ChangesValue()
        {
            // Arrange
            int numThreads = 10;
            CacheNode cacheNode = new CacheNode(1, "value");

            // Assert
            Assert.That(cacheNode.NextNode, Is.Null);

            // Act
            // Tasks run in parallel, tests that the lock ensures no run condition errors
            Parallel.For(0, numThreads, _ =>
            {
                CacheNode newNode = new CacheNode(2, "new value");
                cacheNode.PrevNode = newNode;
                cacheNode.NextNode = newNode;
            });

            // Assert
            Assert.That(cacheNode.NextNode.CacheNodeValue, Is.EqualTo("new value"));
        }

        [Test]
        public void MultiThreadReadsAndWrites_NoException_ChangesValue()
        {
            // Arrange
            int numThreads = 10;
            CacheNode cacheNode = new CacheNode(1, "value");

            // Assert
            Assert.That(cacheNode.NextNode, Is.Null);

            // Act
            // Tasks run in parallel, tests that the lock ensures no run condition errors
            Parallel.For(0, numThreads, _ =>
            {
                CacheNode newNode = new CacheNode(2, "new value");

                // Read
                var prevNode = cacheNode.PrevNode;
                // Write
                cacheNode.NextNode = newNode;
            });

            // Assert
            Assert.That(cacheNode.NextNode.CacheNodeValue, Is.EqualTo("new value"));
        }
    }
}

