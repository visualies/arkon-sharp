using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    class RconAuthenticationException : Exception
    {
        public RconAuthenticationException(string message): base(message)
        {
            
        }
    }
}
