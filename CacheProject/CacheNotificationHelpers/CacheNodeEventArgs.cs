using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProject.CacheNotificationHelpers
{
    /// <summary>
    /// EventArgs for CacheNodeEvictionEventArgs
    /// (The data passed from the event to the event handler)
    /// </summary>
    public class CacheNodeEvictionEventArgs : EventArgs
    {
        public object cacheNodeKey { get; set; }
        public object cacheNodeValue { get; set; }
        public DateTime dateTimeEvicted { get; set; }
    }
}
