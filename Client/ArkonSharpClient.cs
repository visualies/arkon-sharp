using ArkonSharp.Entities;
using ArkonSharp.Exceptions;
using RconSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArkonSharp
{
    public class ArkonSharpClient
    {
        public List<RconConnection> Connections { get; private set; }


        /// <summary>
        /// Registers a new Ark-Server connection to be used by the Client
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <param name="timeout"></param>
        public void AddConnection(string name, string address, int port, string password, int timeout)
        {

            var connection = new RconConnection
            {
                Name = name,
                Address = address,
                RconPort = port,
                Password = password,
                Timeout = timeout
            };

            Connections.Add(connection);
        }

        public async Task<RconClient> ConnectRcon(string ip, int rconPort, string adminPassword, int timeout)
        {
            var client = RconClient.Create(ip, rconPort);

            bool authenticated;
            try
            {
                var task = client.ConnectAsync();

                Thread.Sleep(timeout);

                if (!task.IsCompleted)
                {
                    task.Dispose();
                    throw new RconTimeoutException("Connection timed out");
                }

                authenticated = await client.AuthenticateAsync(adminPassword);
            }
            catch
            {
                //todo
                throw new RconTimeoutException("Connection timed out");
            }

            if (authenticated)
            {
                return client;
            }
            else
            {
                throw new RconAuthenticationException("Failed to authenticate with server");
            }
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
                var client = await ConnectRcon(connection.Address, connection.RconPort, connection.Password, connection.Timeout);
                return await client.ExecuteCommandAsync(command);
            }
            catch (RconTimeoutException)
            {
                throw new RconExecutionException($"Failed to execute command on {connection.Name}");
            }
            catch (RconAuthenticationException)
            {
                throw new RconExecutionException($"Failed to execute command on {connection.Name}");
            }
        }

        /// <summary>
        /// Retrieves a list of all players currently connected to the cluster
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<List<Player>> GetPlayerList()
        {
            var playerlist = new List<Player>();

            foreach (RconConnection connection in Connections)
            {
                var response = await ExecuteCommandAsync(connection, "ListAllPlayerSteamID");
                var lines = response.Split('\n');

                foreach (string line in lines)
                {
                    if (line.Length < 3) continue;

                    var steamId = line.Reverse().TakeWhile(a => char.IsDigit(a)).Reverse();
                    var tribename = line.Reverse().SkipWhile(a => a != ']').TakeWhile(a => a != '[').Skip(1).Reverse();
                    var playername = line.Reverse().SkipWhile(a => a != '[').Skip(2).Reverse();

                    playerlist.Add(new Player(string.Concat(playername), Convert.ToInt64(steamId), string.Concat(tribename)));
                }
            }

            //review: distinct this list ?
            return playerlist;
        }

        public async Task<Player> GetPlayer(long steamId)
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
                        return new Player(string.Concat(playername), steamId, string.Concat(tribename));
                    }
                }
            }

            throw new RconPlayerNotFoundException("Player could not be found on cluster");
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
