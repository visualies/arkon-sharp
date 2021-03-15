using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Exceptions
{
    class ExtendedRconNotInstalledException : Exception
    {
        public ExtendedRconNotInstalledException(string message) : base(message)
        {

        }
    }
}
