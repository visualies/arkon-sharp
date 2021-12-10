using System.Threading.Tasks;

namespace ArkonSharp.Clients
{
    public class WoolyLootboxRconClient : BaseClient
    {
        public WoolyLootboxRconClient(string address, int port, string password) : base(address, port, password)
        {
            
        }
        
        public async Task<string> ReloadPluginAsync()
        {
            var command = $"wlootbox.reload";
            return await ExecuteCommandAsync(command);
        }
    }
}