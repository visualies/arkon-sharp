using ArkonSharp.Entities;
using ArkonSharp.Events;
using ArkonSharp.Exceptions;
using RconSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ArkonSharp
{
    public class ArkonSharpClient
    {
        public List<ArkonConnection> Connections { get; private set; }
        private readonly Timer Timer;
        private readonly ArkonSharpConfiguration ArkonSharpConfiguration;

        /// <summary>
        /// Initialized a new instance of ArkonSharpClient.
        /// </summary>
        /// <param name="config">The Arkon client configuration.</param>
        /// <exception cref="ArkonConfigurationException"></exception>
        public ArkonSharpClient(ArkonSharpConfiguration config)
        {
            if (config.Connections.Count == 0)
            {
                throw new ArkonConfigurationException("Configuration requires at least one connection to be specified");
            }

            ArkonSharpConfiguration = config;
            Connections = config.Connections;
            Timer = new Timer();
        }

        public delegate void PlayerJoinedEventHandler(object source, PlayerJoinedEventArgs args);
        public delegate void PlayerLeftEventHandler(object source, PlayerLeftEventArgs args);
        public delegate void ArkonCacheUpdatedEventHandler(object source, ArkonCacheUpdatedEventArgs args);

        /// <summary>
        /// Fired when a player joins the cluster.
        /// </summary>
        public event PlayerJoinedEventHandler PlayerJoined;
        /// <summary>
        /// Fired when a player leaves the cluster.
        /// </summary>
        public event PlayerLeftEventHandler PlayerLeft;
        /// <summary>
        /// Fired when the client has updated the cache for all connections.
        /// </summary>
        public event ArkonCacheUpdatedEventHandler ArkonCacheUpdated;

        protected virtual void OnPlayerJoined(ArkonPlayer player, string map)
        {
            if (PlayerJoined == null) return;
            PlayerJoined(this, new PlayerJoinedEventArgs()
            {
                Player = player,
                Timestamp = DateTime.Now,
                MapName = map
            });
        }
        protected virtual void OnPlayerLeft(ArkonPlayer player, string map)
        {
            if (PlayerLeft == null) return;
            PlayerLeft(this, new PlayerLeftEventArgs()
            {
                Player = player,
                Timestamp = DateTime.Now,
                MapName = map
            });
        }
        protected virtual void OnOnlinePlayersUpdated(IReadOnlyCollection<ArkonPlayer> onlinePlayers)
        {
            if (ArkonCacheUpdated == null) return;
            ArkonCacheUpdated(this, new ArkonCacheUpdatedEventArgs()
            {
                OnlinePlayers = onlinePlayers,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Gets a Player from cache by their ingame name and map.
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="map">The map the player is on</param>
        /// <returns>The requested player.</returns>
        /// <exception cref="ArkonPlayerNotFoundException">Player with specified name and map not found online</exception>
        public ArkonPlayer GetPlayerFromName(string name, string map)
        {
            var players = GetOnlinePlayers();
            var player = players.FirstOrDefault(a => a.Name == name && a.ServerName == map);
            if (player == null)
            {
                throw new ArkonPlayerNotFoundException("Player with specified name and map not found online");
            }

            return player;
        }

        /// <summary>
        /// Gets a Player from cache by their Steam ID.
        /// </summary>
        /// <param name="steamId">The Steam ID of the player.</param>
        /// <returns>The requested player.</returns>
        /// <exception cref="ArkonPlayerNotFoundException"></exception>
        public ArkonPlayer GetPlayer(long steamId)
        {
            foreach (ArkonConnection connection in Connections)
            {
                var player = connection.ArkonPlayers.FirstOrDefault(a => a.SteamId == steamId);
                if (player != null)
                {
                    return player;
                }
            }
            throw new ArkonPlayerNotFoundException("Player not online");
        }

        /// <summary>
        /// Registers a new Ark-Server connection to be used by the Client
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <param name="timeout"></param>
        public void AddConnection(string mapName, string address, int port, string password, int timeout)
        {
            var connection = new ArkonConnection(mapName, address, port, password, timeout);

            Connections.Add(connection);
        }

        /// <summary>
        /// Starts caching from Servers with the specified interval.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            Timer.Elapsed += async (source, e) =>
            {
                foreach (ArkonConnection connection in Connections)
                {
                    _ = Task.Run(() => OnTimerEvent(connection));
                }
                OnOnlinePlayersUpdated(GetOnlinePlayers());
                await Task.CompletedTask;
            };

            foreach (ArkonConnection connection in Connections)
            {
                _ = Task.Run(() => OnTimerEvent(connection));
            }

            OnOnlinePlayersUpdated(GetOnlinePlayers());

            Timer.Interval = ArkonSharpConfiguration.UpdateInterval;
            Timer.AutoReset = true;
            Timer.Enabled = true;

            await Task.CompletedTask;
        }

        /// <summary>
        /// Executes a command over the specified rcon connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<string> ExecuteCommandAsync(ArkonConnection connection, string command)
        {
            try
            {
                var client = await ConnectRcon(connection.Address, connection.Port, connection.Password, connection.Timeout);
                var response = await client.ExecuteCommandAsync(command);
                return response;
            }
            catch (ArkonTimeoutException e)
            {
                Console.WriteLine(e.Message);
                throw new ArkonExecutionException($"Failed to execute command on {connection.Name}");
            }
            catch (ArkonAuthenticationException e)
            {
                Console.WriteLine(e.Message);
                throw new ArkonExecutionException($"Failed to execute command on {connection.Name}");
            }
        }

        /// <summary>
        /// Retrieves a list of players connected to the cluster from cache.
        /// </summary>
        /// <returns>A collection of online players.</returns>
        public IReadOnlyCollection<ArkonPlayer> GetOnlinePlayers()
        {
            IReadOnlyCollection<ArkonPlayer> playerlist = new List<ArkonPlayer>();

            foreach (ArkonConnection connection in Connections)
            {
                playerlist = playerlist.Concat(connection.ArkonPlayers).ToList();
            }

            return playerlist;
        }

        /// <summary>
        /// Sends a broadcast message visible to all players on the cluster.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <returns></returns>
        public async Task BroadcastAsync(string message)
        {
            foreach (ArkonConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"Broadcast {message}");
            }
        }

        /// <summary>
        /// Sends a broadcast message visible to all players on the map.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <returns></returns>
        public async Task BroadcastAsync(string message, ArkonConnection connection)
        {
            await ExecuteCommandAsync(connection, $"Broadcast {message}");
        }

        /// <summary>
        /// Sends a server chat message visible to all players on the cluster.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <returns></returns>
        public async Task ServerMessage(string message)
        {
            foreach (ArkonConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"ServerChat {message}");
            }
        }

        /// <summary>
        /// Sends a server chat message visible to all players on the map.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <returns></returns>
        public async Task ServerMessage(string message, ArkonConnection connection)
        {
            await ExecuteCommandAsync(connection, $"ServerChat {message}");
        }

        /// <summary>
        /// Sends a client chat message visible to all players on the cluster.
        /// </summary>
        /// <param name="message">The message content to be sent</param>
        /// <param name="author">The name of the sender</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message, string author)
        {
            foreach (ArkonConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"clientchat '{message}' {author}");
            }
        }

        /// <summary>
        /// Retrieves a list of all player locations.
        /// </summary>
        /// <returns>A collection of online player locations.</returns>
        public async Task<IReadOnlyCollection<ArkonPlayerPosition>> GetOnlinePlayerPositions()
        {
            List<ArkonPlayerPosition> positionList = new List<ArkonPlayerPosition>();

            foreach (ArkonConnection connection in Connections)
            {
                var pos = await GetPlayerPosListAsync(connection);
                positionList = positionList.Concat(pos).ToList();
            }

            return positionList;
        }

        internal async Task ServerMessage(ArkonPlayer player, string message)
        {
            try
            {
                char x = '"';
                var con = GetPlayersConnection(player);
                var response = await ExecuteCommandAsync(con, $"ServerChatTo {x}{player.SteamId}{x} {message}");
                Console.WriteLine(response);
            }
            catch (ArkonPlayerNotFoundException)
            {
                throw new ArkonPlayerNotFoundException("Failed to execute command");
            }
        }
        internal async Task AddLootBoxAsync(ArkonPlayer player, string name, int amount)
        {
            try
            {
                var con = GetPlayersConnection(player);
                await ExecuteCommandAsync(con, $"AddLootBox {player.SteamId} {name} {amount}");

            }
            catch (ArkonPlayerNotFoundException)
            {
                throw new ArkonPlayerNotFoundException("Failed to execute command");
            }
        }
        internal async Task<IReadOnlyCollection<ArkonPlayerPosition>> GetPlayerPosListAsync(ArkonConnection connection)
        {
            var positionList = new List<ArkonPlayerPosition>();

            var response = await ExecuteCommandAsync(connection, "ListAllPlayerPos");
            if (response == "\n") return positionList;
            if (response == "Command returned no data\n") throw new ExtendedRconNotInstalledException("This method requires the Extended Rcon Plugin to be installed");

            var lines = response.Split('\n');
            foreach (string line in lines)
            {
                if (line.Length < 3) continue;

                var bracket = ")";
                int bracketCount = (line.Length - line.Replace(bracket, "").Length) / bracket.Length;

                //hpyhen comparison needs an estra string since coords can contain hyphen
                var hypenString = string.Concat(line.Reverse().SkipWhile(a => a != 'X').Reverse());
                var hyphen = "-";
                int hyphenCount = (line.Length - line.Replace(hyphen, "").Length) / hyphen.Length;

                string playername;
                string tribename;

                if (line.Count(f => f == '(') == 1)
                {
                    playername = string.Concat(line.Reverse().SkipWhile(a => a != '(').Skip(4).Reverse());
                    tribename = string.Concat(line.Reverse().SkipWhile(a => a != ')').Skip(1).TakeWhile(a => a != '(').Reverse());
                }
                else if (hypenString.Count(f => f == '-') == 1)
                {
                    playername = string.Concat(hypenString.Reverse().SkipWhile(a => a != '-').Skip(2).Reverse());
                    tribename = string.Concat(hypenString.Reverse().SkipWhile(a => a != ')').Skip(1).TakeWhile(a => a != '-').Reverse().Skip(2));
                }
                else
                {
                    //continue if tribe name and name are impossible to separate 
                    continue;
                }

                //continue if player is "" (player is dead)
                if (string.IsNullOrWhiteSpace(playername)) continue;

                var z = double.Parse(string.Concat(line.Reverse().TakeWhile(a => a != '=').Reverse()), CultureInfo.InvariantCulture);
                var y = double.Parse(string.Concat(line.Reverse().SkipWhile(a => a != 'Z').Skip(1).TakeWhile(a => a != '=').Reverse()), CultureInfo.InvariantCulture);
                var x = double.Parse(string.Concat(line.Reverse().SkipWhile(a => a != 'Y').Skip(1).TakeWhile(a => a != '=').Reverse()), CultureInfo.InvariantCulture);

                var position = new ArkonPlayerPosition(playername, tribename, connection.Name, x, y, z)
                {
                    Client = this
                };

                positionList.Add(position);
            }

            return positionList;
        }
        internal async Task<IReadOnlyCollection<ArkonPlayer>> GetPlayerListAsync(ArkonConnection connection)
        {
            var playerList = new List<ArkonPlayer>();

            var response = await ExecuteCommandAsync(connection, "ListAllPlayerSteamID");
            if (response == "No Players Online\n") return playerList;
            if (response == "Command returned no data\n") throw new ExtendedRconNotInstalledException("This method requires the Extended Rcon Plugin to be installed");
            var lines = response.Split('\n');

            foreach (string line in lines)
            {
                if (line.Length < 3) continue;

                var steamId = line.Reverse().TakeWhile(a => char.IsDigit(a)).Reverse();
                var tribename = line.Reverse().SkipWhile(a => a != ']').TakeWhile(a => a != '[').Skip(1).Reverse();
                var playername = line.Reverse().SkipWhile(a => a != '[').Skip(2).Reverse();


                var player = new ArkonPlayer(string.Concat(playername), Convert.ToInt64(string.Concat(steamId)), string.Concat(tribename), connection.Name)
                {
                    Client = this
                };

                playerList.Add(player);
            }
            Console.WriteLine("count");
            Console.WriteLine(playerList.Count);
            return playerList;
        }
        internal async Task KickAsync(ArkonPlayer player)
        {
            try
            {
                var con = GetPlayersConnection(player);
                await ExecuteCommandAsync(con, $"KickPlayer {player.SteamId}");
            }
            catch (ArkonPlayerNotFoundException)
            {
                throw new ArkonPlayerNotFoundException("Failed to execute command");
            }

        }
        internal async Task BanAsync(ArkonPlayer player, string reason)
        {
            foreach (ArkonConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"BanPlayer {player.SteamId} {reason}");
            }
        }
        internal async Task AddPointsAsync(ArkonPlayer player, int amount)
        {
            try
            {
                var con = GetPlayersConnection(player);
                var response = await ExecuteCommandAsync(con, $"AddPoints {player.SteamId} {amount}");
            }
            catch (ArkonPlayerNotFoundException)
            {
                throw new ArkonPlayerNotFoundException("Failed to execute command");
            }
        }

        private async Task<RconClient> ConnectRcon(string ip, int rconPort, string adminPassword, int timeout)
        {
            var client = RconClient.Create(ip, rconPort);
            await client.ConnectAsync();

            bool authenticated = await client.AuthenticateAsync(adminPassword);

            if (authenticated)
            {
                return client;
            }
            else
            {
                throw new ArkonAuthenticationException("Failed to authenticate with server");
            }
        }
        private async Task OnTimerEvent(ArkonConnection connection)
        {
            IEnumerable<ArkonPlayer> players = await GetPlayerListAsync(connection);

            List<ArkonPlayer> joinedPlayers;
            List<ArkonPlayer> leftPlayers;

            joinedPlayers = players.Except(connection.ArkonPlayers, new ArkonPlayerComparer()).ToList();
            leftPlayers = connection.ArkonPlayers.Except(players, new ArkonPlayerComparer()).ToList();

            foreach (ArkonPlayer joined in joinedPlayers)
            {
                OnPlayerJoined(joined, connection.Name);
            }

            foreach (ArkonPlayer left in leftPlayers)
            {
                OnPlayerLeft(left, connection.Name);
            }

            connection.ArkonPlayers = players;
        }

        private ArkonConnection GetPlayersConnection(ArkonPlayer player)
        {
            var con = Connections.FirstOrDefault(a => a.ArkonPlayers.Contains(player, new ArkonPlayerComparer()));
            if (con == null)
            {
                throw new ArkonPlayerNotFoundException("Player not found in connections");
            }
            return con;
        }

    }
}
