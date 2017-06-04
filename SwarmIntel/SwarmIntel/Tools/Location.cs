using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmIntel.Tools
{
    public class Location
    {
        public int Id;
        public int X;
        public int Y;
        public int Demand;
        public int CurrentSupply;
        public Location(int id, int x,int y,int demand)
        {
            Id = id;
            X = x;
            Y = y;
            Demand = demand;
            CurrentSupply = 0;
        }
        public Location(Location location)
        {
            Id = location.Id;
            X = location.X;
            Y = location.Y;
            Demand = location.Demand;
            CurrentSupply = location.CurrentSupply;
        }
        //public static bool operator ==(Location leftLocation, Location rightLocation)
        //{
        //    return (leftLocation.X == rightLocation.X && leftLocation.Y == rightLocation.Y && leftLocation.Demand == rightLocation.Demand);
        //}
        //public static bool operator !=(Location leftLocation, Location rightLocation)
        //{
        //    return !(leftLocation == rightLocation);
        //}
    }
}
