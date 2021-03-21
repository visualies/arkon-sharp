using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    public class ArkonTimeoutException : Exception
    {
        public ArkonTimeoutException(string message): base(message)
        {

        }
    }
}
