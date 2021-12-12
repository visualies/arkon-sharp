using System.Threading.Tasks;
using ArkonSharp.Clients;

namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayerOnline : Player
    {
        public string PlayerName { get; }
        public string TribeName { get; }

        public ExtendedRconPlayerOnline(long steamId, string playerName, string tribeName) : base(steamId)
        {
            PlayerName = playerName;
            TribeName = tribeName;
        }

    }
}