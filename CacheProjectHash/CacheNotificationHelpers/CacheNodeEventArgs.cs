using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProjectHash.CacheNotificationHelpers
{
    /// <summary>
    /// EventArgs for CacheNodeEvictionEventArgs
    /// (The data passed from the event to the event handler)
    /// </summary>
    public class CacheNodeEvictionEventArgs<TCacheNodeKey, TCacheNodeValue> : EventArgs
    {
        public TCacheNodeKey cacheNodeKey { get; set; }
        public TCacheNodeValue cacheNodeValue { get; set; }
        public DateTime dateTimeEvicted { get; set; }
    }
}
