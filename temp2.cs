using NUnit.Framework;


[TestFixture]
public class CacheNodeTests
{
    [Test]
    public void CreateCacheNodeStringValue()
    {
        // Arrange
        int key = 1;
        string value = "Value";

        // Act
        CacheNode cacheNode = new CacheNode<int, string>(key, value);

        // Assert
        Assert.AreEqual(key, cacheNode.Key);
        Assert.AreEqual(value, cacheNode.Value);
        Assert.IsNull(cacheNode.PrevNode);
        Assert.IsNull(cacheNode.NextNode);
    }

    [Test]
    public void CreateCacheNodeIntValue()
    {
        // Arrange
        int key = 1;
        int value = 2;

        // Act
        CacheNode cacheNode = new CacheNode<int, int>(key, value);

        // Assert
        Assert.AreEqual(key, cacheNode.Key);
        Assert.AreEqual(value, cacheNode.Value);
        Assert.IsNull(cacheNode.PrevNode);
        Assert.IsNull(cacheNode.NextNode);
    }

    [Test]
    public void CreateCacheNodeBoolValue()
    {
        // Arrange
        int key = 1;
        bool value = true;

        // Act
        CacheNode cacheNode = new CacheNode<int, bool>(key, value);

        // Assert
        Assert.AreEqual(key, cacheNode.Key);
        Assert.AreEqual(value, cacheNode.Value);
        Assert.IsNull(cacheNode.PrevNode);
        Assert.IsNull(cacheNode.NextNode);
    }

    [Test]
    public void CreateCacheNodeClassValue()
    {
        // Arrange
        private class TestClass
        {
            public string Data { get; }

            public TestClass(string Data)
            {
                this.Data = Data;
            }
        }
        int key = 1;
        TestClass value = new TestClass("TestValue");

        // Act
        CacheNode cacheNode = new CacheNode<int, TestClass>(key, value);

        // Assert
        Assert.AreEqual(key, cacheNode.Key);
        Assert.AreEqual(value, cacheNode.Value);
        Assert.IsNull(cacheNode.PrevNode);
        Assert.IsNull(cacheNode.NextNode);
    }
}