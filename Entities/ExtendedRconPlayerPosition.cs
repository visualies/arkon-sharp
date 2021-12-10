using System.Numerics;
using ArkonSharp.Clients;

namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayerPosition : Entity
    {
        
        public readonly string PlayerName;
        public readonly string TribeName;
        public readonly Vector3 Position;
        
        public ExtendedRconPlayerPosition(Vector3 position, string playerName, string tribeName)
        {
            Position = position;
            PlayerName = playerName;
            TribeName = tribeName;
        }
    }
}