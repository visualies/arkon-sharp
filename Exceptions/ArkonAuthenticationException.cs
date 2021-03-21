using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    public class ArkonAuthenticationException : Exception
    {
        public ArkonAuthenticationException(string message): base(message)
        {
            
        }
    }
}
