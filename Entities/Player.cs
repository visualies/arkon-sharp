using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Entities
{
    internal class Player : Entity
    {
        public long SteamId { get; }

        public Player(long steamId)
        {
            SteamId = steamId;
        }
    }
}
