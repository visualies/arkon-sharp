using System;
using System.Collections.Generic;
using System.Text;

namespace ArkonSharp.Entities
{
    public class ArkonPlayerPosition : Entity
    {
        public ArkonPlayerPosition(string name, string tribeName, string mapName, double x, double y, double z)
        {
            Name = name;
            TribeName = tribeName;
            MapName = mapName;
            Timestamp = DateTime.Now;
            X = x;
            Y = y;
            Z = z;
        }
        public string Name { get; set; }
        public string TribeName { get; set; }
        public string MapName { get; set; }
        public DateTime Timestamp { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
