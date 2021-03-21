using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    public class ArkonExecutionException : Exception
    {
        public ArkonExecutionException(string message): base(message)
        {
            
        }
    }
}
