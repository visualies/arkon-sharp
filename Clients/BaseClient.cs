using System;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;
using ArkonSharp.Entities;
using RconSharp;

namespace ArkonSharp.Clients
{
    public abstract class BaseClient
    {
        private readonly string _address;
        private readonly int _port;
        private readonly string _password;

        protected BaseClient(string address, int port, string password)
        {
            _address = address;
            _port = port;
            _password = password;
        }
        protected async Task<string> ExecuteCommandAsync( string command)
        {
            var client = RconClient.Create(_address, _port);
            await client.ConnectAsync();

            if (!await client.AuthenticateAsync(_password))
            {
                throw new AuthenticationException();
            }

            return await client.ExecuteCommandAsync(command);
        }
    }
}