using System;
using System.Collections.Generic;

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

        public static float GetDistance(Location location1, Location location2)
        {
            return (float) Math.Sqrt(Math.Pow(Math.Abs(location1.X - location2.X), 2) 
                    + Math.Pow(Math.Abs(location1.Y - location2.Y), 2));
        }

        public static Location GetNearestCity(Location location, List<Location> cities)
        {
            Location nearestCity = null;
            int minDistance = Int32.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < cities.Count; i++)
            {
                var city = cities[i];
                var distance = Location.GetDistance(location, city);
                if (minDistance > distance)
                {
                    nearestCity = new Location(city);
                    minIndex = i;
                }
            }
            if (nearestCity != null) cities.RemoveAt(minIndex);
            return nearestCity;
        }
    }
}