using System.Numerics;

namespace ArkonSharp.Entities
{
    public class ExtendedRconPlayerPosition : Entity
    {
        public string PlayerName { get; }
        public Vector3 Position { get; }
        public string TribeName { get; }

        public ExtendedRconPlayerPosition(Vector3 position, string playerName, string tribeName)
        {
            Position = position;
            PlayerName = playerName;
            TribeName = tribeName;
        }
    }
}