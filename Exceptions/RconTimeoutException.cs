using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    public class RconTimeoutException : Exception
    {
        public RconTimeoutException(string message): base(message)
        {

        }
    }
}
