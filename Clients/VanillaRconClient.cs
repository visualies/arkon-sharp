using System.Threading.Tasks;

namespace ArkonSharp.Clients
{
    public class VanillaRconClient : BaseClient
    {
        public VanillaRconClient(string address, int port, string password) : base(address, port, password)
        {
        }

        public async Task<string> SendCustomCommandAsync(string input)
        {
            var command = $"{input}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendServerMessageToPlayerAsync(long steamId, string message)
        {
            var command = $"ServerChatTo \"{steamId}\" {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendServerMessageAsync(string message)
        {
            var command = $"ServerChat {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendBroadcastMessageAsync(string message)
        {
            var command = $"Broadcast {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> SendTribeLogMessageAsync(long tribeId, string message)
        {
            var command = $"TribeLogMsg {tribeId} {message}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> KickPlayerAsync(long steamId)
        {
            var command = $"KickPlayer {steamId}";
            return await ExecuteCommandAsync(command);
        }

        public async Task<string> BanPlayerAsync(long steamId, string reason)
        {
            var command = $"BanPlayer {steamId} {reason}";
            return await ExecuteCommandAsync(command);
        }
    }
}