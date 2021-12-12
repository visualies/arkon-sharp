namespace ArkonSharp.Entities
{
    public class PlayerEntiy : Entity
    {
        public readonly long SteamId;

        public PlayerEntiy(long steamId)
        {
            SteamId = steamId;
        }
    }
}