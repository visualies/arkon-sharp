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
            var rcon = new ArkonSharpClient("12.34.567.89", 32330, "adminpass");
            
            //ArkShop Example: <steamId> <points>
            rcon.ArkShop.AddPointsAsync(76561198223679884, 500)
            
            //WLootbox Example: <steamId> <boxKeyword> <amount>
            rcon.WoolyLootbox.AddLootboxAsync(76561198223679884, "Gold", 5);
        }
    }
}
```
Implements [RconSharp](https://github.com/stefanodriussi/rconsharp)

#### Nuget Package .NET CLI
```cli
dotnet add package ArkonSharp --version 1.2.0
```
