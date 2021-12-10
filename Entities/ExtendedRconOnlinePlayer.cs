using ArkonSharp.Clients;

namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayerOnline : Entity
    {
        public readonly string PlayerName;
        public readonly string TribeName;
        public readonly long SteamId;

        public ExtendedRconPlayerOnline(long steamId, string playerName, string tribeName)
        {
            SteamId = steamId;
            PlayerName = playerName;
            TribeName = tribeName;
        }
    }
}