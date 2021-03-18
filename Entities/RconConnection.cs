using ArkonSharp.Entities;
using System.Collections.Generic;

namespace ArkonSharp
{
    public class RconConnection
    {
        public readonly string Name;
        public readonly string Address;
        public readonly int Port;
        public readonly string Password;
        public readonly int Timeout;
        public List<ArkonPlayer> ArkonPlayers { get; set; }
        public List<ArkonPlayerPosition> ArkonPlayerPositions { get; set; }

        public RconConnection(string name, string address, int port, string password, int timeout)
        {
            Name = name;
            Address = address;
            Port = port;
            Password = password;
            Timeout = timeout;

            ArkonPlayers = new List<ArkonPlayer>();
            ArkonPlayerPositions = new List<ArkonPlayerPosition>();
        }
    }

}