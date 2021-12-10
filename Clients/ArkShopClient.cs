using System.Threading.Tasks;

namespace ArkonSharp.Clients
{
    public class ArkShopClient : BaseClient
    {
        public ArkShopClient(string address, int port, string password) : base(address, port, password)
        {
        }

        public async Task<string> AddPointsAsync(long steamId, int amount)
        {
            var command = $"AddPoints {steamId} {amount}";
            return await ExecuteCommandAsync(command);
        }
    }
}