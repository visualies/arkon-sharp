using System.Collections.Generic;

namespace ArkonSharp
{
    public class ArkonSharpConfiguration
    {
        public int UpdateInterval { get; set; }
        public List<ArkonConnection> Connections { get; set; }
    }
}