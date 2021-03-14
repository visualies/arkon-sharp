using ArkonSharp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Events
{
    public class PlayerJoinedEventArgs
    {
        public ArkonPlayer Player { get; set; }
        public DateTime Timestamp { get; set; }
        public string MapName { get; set; }
    }
}
