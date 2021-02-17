using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    class RconExecutionException : Exception
    {
        public RconExecutionException(string message): base(message)
        {
            
        }
    }
}
