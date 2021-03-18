using ArkonSharp.Entities;
using ArkonSharp.Events;
using ArkonSharp.Exceptions;
using RconSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace ArkonSharp
{
    public class ArkonSharpClient
    {
        public List<RconConnection> Connections { get; private set; }
        private readonly Timer Timer;
        private readonly ArkonSharpConfiguration ArkonSharpConfiguration;

        public ArkonSharpClient(ArkonSharpConfiguration config)
        {
            ArkonSharpConfiguration = config;
            Connections = new List<RconConnection>();
            Timer = new Timer();
        }

        public delegate void PlayerJoinedEventHandler(object source, PlayerJoinedEventArgs args);
        public delegate void PlayerLeftEventHandler(object source, PlayerLeftEventArgs args);
        public delegate void OnlinePlayersUpdatedHandler(object source, OnlinePlayersUpdateEventArgs args);

        public event PlayerJoinedEventHandler PlayerJoined;
        public event PlayerLeftEventHandler PlayerLeft;
        public event OnlinePlayersUpdatedHandler OnlinePlayersUpdated;
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
        protected virtual void OnOnlinePlayersUpdated(List<ArkonPlayer> onlinePlayers)
        {
            if (OnlinePlayersUpdated == null) return;
            OnlinePlayersUpdated(this, new OnlinePlayersUpdateEventArgs()
            {
                OnlinePlayers = onlinePlayers,
                Timestamp = DateTime.Now
            });
        }

        public async Task<ArkonPlayer> GetPlayer(long steamId)
        {
            foreach (RconConnection connection in Connections)
            {
                var response = await ExecuteCommandAsync(connection, "ListAllPlayerSteamID");
                var lines = response.Split('\n');

                foreach (string line in lines)
                {
                    var searchSteamId = line.Reverse().TakeWhile(a => char.IsDigit(a)).Reverse();
                    var tribename = line.Reverse().SkipWhile(a => a != ']').TakeWhile(a => a != '[').Skip(1).Reverse();
                    var playername = line.Reverse().SkipWhile(a => a != '[').Skip(2).Reverse();

                    if (Convert.ToInt64(searchSteamId) == steamId)
                    {
                        var player = new ArkonPlayer(string.Concat(playername), Convert.ToInt64(steamId), string.Concat(tribename), connection.Name)
                        {
                            Client = this
                        };

                        return player;
                    }
                }
            }

            throw new RconPlayerNotFoundException("Player could not be found on cluster");
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
            var connection = new RconConnection(mapName, address, port, password, timeout);

            Connections.Add(connection);
        }
        /// <summary>
        /// Starts the live link connection to the ark servers.
        /// </summary>
        /// <param name="updateInterval"></param>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            Timer.Elapsed += async (source, e) =>
            {
                foreach (RconConnection connection in Connections)
                {
                    _ = Task.Run(() => OnTimerEvent(source, e, connection));
                }
                OnOnlinePlayersUpdated(GetOnlinePlayers());
                await Task.CompletedTask;
            };

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
        public async Task<string> ExecuteCommandAsync(RconConnection connection, string command)
        {
            try
            {
                var client = await ConnectRcon(connection.Address, connection.Port, connection.Password, connection.Timeout);
                var response = await client.ExecuteCommandAsync(command);
                return response;
            }
            catch (RconTimeoutException e)
            {
                Console.WriteLine(e.Message);
                throw new RconExecutionException($"Failed to execute command on {connection.Name}");
            }
            catch (RconAuthenticationException e)
            {
                Console.WriteLine(e.Message);
                throw new RconExecutionException($"Failed to execute command on {connection.Name}");
            }
        }
        /// <summary>
        /// Retrieves a list of all players currently connected to the cluster
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<ArkonPlayer> GetOnlinePlayers()
        {
            List<ArkonPlayer> playerlist = new List<ArkonPlayer>();

            foreach (RconConnection connection in Connections)
            {
                playerlist = playerlist.Concat(connection.ArkonPlayers).ToList();
            }

            return playerlist;
        }
        public async Task<List<ArkonPlayerPosition>> GetOnlinePlayerPositions()
        {
            List<ArkonPlayerPosition> positionList = new List<ArkonPlayerPosition>();

            foreach (RconConnection connection in Connections)
            {
                var pos = await GetPlayerPosListAsync(connection);
                positionList = positionList.Concat(pos).ToList();
            }

            return positionList;
        }
        internal async Task<List<ArkonPlayerPosition>> GetPlayerPosListAsync(RconConnection connection)
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
                    playername = string.Concat(line.Reverse().SkipWhile(a => a != '(').Skip(3).Reverse());
                    tribename = string.Concat(line.Reverse().SkipWhile(a => a != ')').Skip(1).TakeWhile(a => a != '(').Reverse());
                }
                else if (hypenString.Count(f => f == '-') == 1)
                {
                    playername = string.Concat(hypenString.Reverse().SkipWhile(a => a != '-').Skip(1).Reverse());
                    tribename = string.Concat(hypenString.Reverse().SkipWhile(a=> a != ')').Skip(1).TakeWhile(a => a != '-').Reverse().Skip(2));
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
        internal async Task<List<ArkonPlayer>> GetPlayerListAsync(RconConnection connection)
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

            return playerList;
        }
        internal async Task KickAsync(ArkonPlayer player)
        {
            foreach (RconConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"KickPlayer {player.SteamId}");
            }
        }
        internal async Task BanAsync(ArkonPlayer player, string reason)
        {
            foreach (RconConnection connection in Connections)
            {
                await ExecuteCommandAsync(connection, $"BanPlayer {player.SteamId} {reason}");
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
                throw new RconAuthenticationException("Failed to authenticate with server");
            }
        }
        private async Task OnTimerEvent(object source, ElapsedEventArgs e, RconConnection connection)
        {
            List<ArkonPlayer> players = await GetPlayerListAsync(connection);

            List<ArkonPlayer> joinedPlayers = new List<ArkonPlayer>();
            List<ArkonPlayer> leftPlayers = new List<ArkonPlayer>();

            joinedPlayers = players.Except(connection.ArkonPlayers, new ArkonPlayerComparer()).ToList();
            leftPlayers = connection.ArkonPlayers.Except(players, new ArkonPlayerComparer()).ToList();
            //todo NullException

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

        private async Task UpdateOnlinePlayersAsync()
        {

        }

        //public async Task GetTribeLog(string playerlist)
        //{
        //    var lines = playerlist.Split('\n');


        //    foreach (string line in lines)
        //    {

        //    }
        //}
    }
}
