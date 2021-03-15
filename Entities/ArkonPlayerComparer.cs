using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Entities
{
    public class ArkonPlayerComparer : IEqualityComparer<ArkonPlayer>
    {
        public bool Equals(ArkonPlayer x, ArkonPlayer y)
        {
            if (x.SteamId == y.SteamId)
                return true;

            return false;
        }

        public int GetHashCode(ArkonPlayer obj)
        {
            return obj.SteamId.GetHashCode();
        }
    }
}
