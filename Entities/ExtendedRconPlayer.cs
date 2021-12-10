using System;
using System.Threading.Tasks;
using ArkonSharp.Clients;
using ArkonSharp.Enums;

namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayer : Entity
    {
        public readonly long SteamId;

        public ExtendedRconPlayer(long steamId)
        {
            this.SteamId = steamId;
        }
    }
}