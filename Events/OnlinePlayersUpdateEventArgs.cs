using ArkonSharp.Entities;
using System;
using System.Collections.Generic;

namespace ArkonSharp
{
    public class ArkonCacheUpdatedEventArgs
    {
        public IReadOnlyCollection<ArkonPlayer> OnlinePlayers { get; set; }
        public DateTime Timestamp { get; set; }
    }
}