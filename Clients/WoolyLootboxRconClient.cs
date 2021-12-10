using System.Threading.Tasks;

namespace ArkonSharp.Clients
{
    public class WoolyLootboxRconClient : BaseClient
    {
        public WoolyLootboxRconClient(string address, int port, string password) : base(address, port, password)
        {
            
        }
        
        public async Task<string> AddLootboxAsync(long steamId, string boxKeyword, int amount, int daysOpenDelay, int hoursOpenDelay, int minutesOpenDelay)
        {
            var command = $"WLootBox.GiveLootBoxItem {steamId} {boxKeyword} {amount} {daysOpenDelay} {hoursOpenDelay} {minutesOpenDelay}";
            return await ExecuteCommandAsync(command);
        }
        
        public async Task<string> ChangeLootboxAmountAsync(long steamId, string boxKeyword, int amount)
        {
            var command = $"WLootBox.ChangeBoxAmount {steamId} {boxKeyword} {amount}";
            return await ExecuteCommandAsync(command);
        }
        
        public async Task<string> ReloadPluginAsync()
        {
            var command = $"wlootbox.reload";
            return await ExecuteCommandAsync(command);
        }
        
    }
}