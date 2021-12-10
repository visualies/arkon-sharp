namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayerOnline : Entity
    {
        public readonly string PlayerName;
        public readonly long SteamId;
        public readonly string TribeName;

        public ExtendedRconPlayerOnline(long steamId, string playerName, string tribeName)
        {
            SteamId = steamId;
            PlayerName = playerName;
            TribeName = tribeName;
        }
    }
}