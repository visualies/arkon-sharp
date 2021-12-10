# ArkonSharp

Rcon Library to work with Ark Survival Evolved commands.

```csharp
using ArkonSharp;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a new ArkonSharpClient with <address> <rconPort> and <rconPassword>
            var rcon = new ArkonSharpClient("12.34.567.89", 32330, "adminpass");
            
            //Examples
            
            //ArkShop give points to player <steamId> <points>
            await rcon.ArkShop.AddPointsAsync(76561198223679884, 500)
            
            //WLootbox give lootbox to player <steamId> <boxKeyword> <amount>
            await rcon.WoolyLootbox.AddLootboxAsync(76561198223679884, "Gold", 5);
            
            //Get a playerlist to easily iterate through online players
            List<ExtendedRconPlayer> players = await rcon.ExtendedRcon.GetOnlinePlayersAsync();
        }
    }
}
```

Implements [RconSharp](https://github.com/stefanodriussi/rconsharp)

Supported Plugins:

[AdvancedDinoColor](https://arkserverapi.com/index.php?resources/advanced-dino-color.152/) [Implemented]

[ArkShop](https://arkserverapi.com/index.php?resources/shop-currency-kits.14/) [Implemented]

[ExtendedRcon](https://arkserverapi.com/index.php?resources/extended-rcon.5/) [Partially Implemented]

[WoolyLootbox]() [Implemented]

#### Nuget Package .NET CLI

```cli
dotnet add package ArkonSharp --version 3.0.0
```
