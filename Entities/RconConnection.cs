using ArkonSharp.Entities;
using System.Collections.Generic;

namespace ArkonSharp
{
    public class RconConnection
    {
        public readonly string Name;
        public readonly string Address;
        public readonly int RconPort;
        public readonly string Password;
        public readonly int Timeout;

        public RconConnection()
        {

        }
        public List<ArkonPlayer> ArkonPlayers { get; private set; }
    }

}