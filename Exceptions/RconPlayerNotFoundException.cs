using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    class RconPlayerNotFoundException : Exception
    {
        public RconPlayerNotFoundException(string message): base(message)
        {
            
        }
    }
}
