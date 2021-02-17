# Rcon Library to work with Ark Survival Evolved commands

#Implements RconSharp
```
using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var rcon = new ArkonSharpClient();
            
            rcon.AddConnection("name", "12.34.567.89", 32330, "adminpass", 3)
            rcon.AddConnection("name", "12.34.567.89", 32332, "adminpass", 3)
            
            var players = rcon.GetOnlinePlayers();
        }
    }
}
```
