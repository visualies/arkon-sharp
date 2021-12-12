using ArkonSharp.Clients;

namespace ArkonSharp
{
    public class ArkonSharpClient
    {
        public readonly ArkShopClient ArkShop;
        public readonly ExtendedRconClient ExtendedRcon;
        public readonly VanillaRconClient VanillaRcon;
        public readonly WoolyLootboxRconClient WoolyLootboxRcon;
        public readonly AdvancedDinoColorClient AdvancedDinoColor;

        public ArkonSharpClient(string address, int port, string password)
        {
            AdvancedDinoColor = new AdvancedDinoColorClient(address, port, password);
            ArkShop = new ArkShopClient(address, port, password);
            ExtendedRcon = new ExtendedRconClient(address, port, password);
            VanillaRcon = new VanillaRconClient(address, port, password);
            WoolyLootboxRcon = new WoolyLootboxRconClient(address, port, password);
        }
    }
}