using System.Threading.Tasks;

namespace ArkonSharp.Clients
{
    public class AdvancedDinoColorClient : BaseClient
    {
        public AdvancedDinoColorClient(string address, int port, string password) : base(address, port, password)
        {
        }
        
        public async Task<string> AddTokenAsync(long steamId, int amount)
        {
            var command = $"ADC.add {steamId} {amount}";
            return await ExecuteCommandAsync(command);
        }
        
        public async Task<string> ReloadPluginAsync()
        {
            var command = "adc.reload";
            return await ExecuteCommandAsync(command);
        }
    }
}