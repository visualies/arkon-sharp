using ArkonSharp.Exceptions;
using RconSharp;
using System;
using System.Collections.Generic;
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
        public async Task<List<long>> GetOnlinePlayers()
        {
            List<long> onlinePlayers = new List<long>();

            foreach (RconConnection connection in Connections)
            {
                try
                {
                    var result = await ExecuteCommandAsync(connection, "ListAllPlayerSteamID");
                }
                catch (RconExecutionException)
                {
                    throw;
                }
            }

            return onlinePlayers;
        }

        public async Task ParsePlayerList(string playerlist)
        {
            var lines = playerlist.Split('\n');

            foreach (string line in lines)
            {

            }
        }
    }
}
