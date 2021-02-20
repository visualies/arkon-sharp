using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ArkonSharp.Entities
{
    public class Player : ArkonSharpClient
    {
        public Player(string name, long steamid, string tribename)
        {
            Name = name;
            SteamId = steamid;
            TribeName = tribename;
        }

        public string Name { get; private set; }
        public long SteamId { get; private set; }
        public string TribeName { get; private set; }


        public async Task Kick()
        {
            foreach (RconConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"KickPlayer {SteamId}");
            }
        }

        public async Task Ban(string reason = null)
        {
            foreach (RconConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"BanPlayer {SteamId} {reason}");
            }
        }
    }
}
