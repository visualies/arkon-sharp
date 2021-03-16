using ArkonSharp.Entities;
using System;
using System.Collections.Generic;

namespace ArkonSharp
{
    public class OnlinePlayersUpdateEventArgs
    {
        public List<ArkonPlayer> OnlinePlayers { get; set; }
        public DateTime Timestamp { get; set; }
    }
}