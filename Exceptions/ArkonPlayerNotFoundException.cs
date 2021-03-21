using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    public class ArkonPlayerNotFoundException : Exception
    {
        public ArkonPlayerNotFoundException(string message): base(message)
        {
            
        }
    }
}
