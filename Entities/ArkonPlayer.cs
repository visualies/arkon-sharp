using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ArkonSharp.Entities
{
    public class ArkonPlayer : Entity
    {
        public ArkonPlayer(string name, long steamid, string tribename, string serverName)
        {
            Name = name;
            SteamId = steamid;
            TribeName = tribename;
            ServerName = serverName;
        }

        public string Name { get; private set; }
        public long SteamId { get; private set; }
        public string TribeName { get; private set; }
        public string ServerName { get; set; }


        public async Task Kick()
        {
            await Client.KickAsync(this);
        }

        public async Task Ban(string reason = null)
        {
            await Client.BanAsync(this, reason);
        }

        /// <summary>
        /// Retrives a list of all tribes this player is part of
        /// </summary>
        /// <returns></returns>
    }
}
