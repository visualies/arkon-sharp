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

        /// <summary>
        /// Kicks the player from the cached connection he is registered on.
        /// </summary>
        /// <returns></returns>
        public async Task Kick()
        {
            await Client.KickAsync(this);
        }

        /// <summary>
        /// Bans the player from all connections.
        /// </summary>
        /// <returns></returns>
        public async Task Ban(string reason = null)
        {
            await Client.BanAsync(this, reason);
        }

        /// <summary>
        /// Sends a server chat message visible to the player, on the cached connection he is registered on.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            await Client.ServerMessage(this, message);
        }

        /// <summary>
        /// Adds lootboxes+ to a players account.
        /// </summary>
        /// <param name="lootbox">The name of the lootbox.</param>
        /// <param name="amount">The amount of lootboxes to add.</param>
        /// <returns></returns>
        public async Task AddLootBoxAsync(string lootbox, int amount)
        {
            await Client.AddLootBoxAsync(this, lootbox, amount);
        }

        /// <summary>
        /// Adds arkshop points to a players balance.
        /// </summary>
        /// <param name="amount">The amount of points to add.</param>
        /// <returns></returns>
        public async Task AddPointsAsync(int amount)
        {
            await Client.AddPointsAsync(this, amount);
        }
    }
}
