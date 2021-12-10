namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayer : Entity
    {
        public readonly long SteamId;

        public ExtendedRconPlayer(long steamId)
        {
            SteamId = steamId;
        }
    }
}