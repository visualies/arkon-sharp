using System.Numerics;

namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayerPosition : Entity
    {
        public readonly string PlayerName;
        public readonly Vector3 Position;
        public readonly string TribeName;

        public ExtendedRconPlayerPosition(Vector3 position, string playerName, string tribeName)
        {
            Position = position;
            PlayerName = playerName;
            TribeName = tribeName;
        }
    }
}