using System.Security.Authentication;
using System.Threading.Tasks;
using RconSharp;

namespace ArkonSharp.Clients
{
    public abstract class BaseClient
    {
        private readonly string _address;
        private readonly string _password;
        private readonly int _port;

        protected BaseClient(string address, int port, string password)
        {
            _address = address;
            _port = port;
            _password = password;
        }

        protected async Task<string> ExecuteCommandAsync(string command)
        {
            var client = RconClient.Create(_address, _port);
            await client.ConnectAsync();

            if (!await client.AuthenticateAsync(_password)) throw new AuthenticationException();

            return await client.ExecuteCommandAsync(command);
        }
    }
}