namespace ArkonSharp.Clients
{
    public class ArkonSharpClient
    {
        public readonly ExtendedRconClient ExtendedRcon;
        public readonly VanillaRconClient VanillaRcon;
        public readonly WoolyLootboxRconClient WoolyLootboxRcon;
        public readonly ArkShopClient ArkShop;
        
        public ArkonSharpClient(string address, int port, string password)
        {
            ArkShop = new ArkShopClient(address, port, password);
            ExtendedRcon = new ExtendedRconClient(address, port, password);
            VanillaRcon = new VanillaRconClient(address, port, password);
            WoolyLootboxRcon = new WoolyLootboxRconClient(address, port, password);
        }
    }
}