using System;
using System.Collections.Generic;
using System.Threading;

namespace CacheProject
{
    class Program
    {
        static void Main()
        {
            // Create a dictionary with CacheNodeKey as keys and CacheNode instances as values
            Dictionary<CacheNodeKey, CacheNode<CacheNodeKey, CacheNodeValue>> cacheDictionary =
                new Dictionary<CacheNodeKey, CacheNode<CacheNodeKey, CacheNodeValue>>();

        }
    }

    // Define CacheNodeKey and CacheNodeValue classes if not already defined
    public class CacheNodeKey
    {
        public string Key { get; }

        public CacheNodeKey(string key)
        {
            Key = key;
        }
    }

    public class CacheNodeValue
    {
        public string Value { get; }

        public CacheNodeValue(string value)
        {
            Value = value;
        }
    }
}
