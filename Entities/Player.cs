using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Entities
{
    public class Player : Entity
    {
        public long SteamId { get; }

        public Player(long steamId)
        {
            SteamId = steamId;
        }
    }
}
