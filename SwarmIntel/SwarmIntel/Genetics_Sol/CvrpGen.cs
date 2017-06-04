using System;
using System.Collections.Generic;
using System.Linq;
using SwarmIntel.GeneticsAlgorithm;
using SwarmIntel.Tools;

namespace SwarmIntel.Genetics_Sol
{
    class CvrpGen : Gen
    {
        public List<Vehicle> Vehicles;
        public int NumOfVehicles;
        public double Cost;

        public CvrpGen(int numOfVehicles, List<Location> locations)
        {
            Cost = 0;
            NumOfVehicles = numOfVehicles;
            Vehicles= new List<Vehicle>();
            for (int i = 1; i <= NumOfVehicles; i++)
            {
                Vehicles.Add(new Vehicle(i));
            }
            SupplyDemands(locations);
        }

        public void SupplyDemands(List<Location> locations)
        {
            List<Location> cities = new List<Location>();
            foreach (Location location in locations)
            {
                cities.Add(new Location(location));
            }
            cities.RemoveAt(0); //remove warehouse from cities
            foreach (Vehicle vehicle in Vehicles)
            {
                int i = 0;
                while (vehicle.SupplyLeft > 0 && i< cities.Count)
                {
                    if (vehicle.Route.Count > 22)
                    {
                        var d = 90;
                    }
                    if (cities[i].Demand <= (vehicle.SupplyLeft))
                    {
                        DeliverSuplyToCity(cities[i], vehicle);
                        vehicle.Route.Add(cities[i].Id);
                        cities.RemoveAt(i);
                    }
                    else
                    {
                        i++;                        
                    }
                }
            }
        }

        private void DeliverSuplyToCity(Location city,Vehicle vehicle)
        {
            if (city.Demand - city.CurrentSupply < vehicle.SupplyLeft)
            {
                vehicle.SupplyLeft -= (city.Demand- city.CurrentSupply);
                city.CurrentSupply = city.Demand;
            }
            else
            {
                city.CurrentSupply += vehicle.SupplyLeft;
                vehicle.SupplyLeft = 0;
            }            
        }

        public void CalcCost()
        {
            Cost = 0;
            foreach (Vehicle vehicle in Vehicles)
            {
                var route = vehicle.Route;
                if (route.Count > 0)
                {
                    // calc first ride from warehouse
                    Cost += GetDistance(CvrpGenetics.CvrProbelm.Locations.First(), CvrpGenetics.CvrProbelm.Locations[route.First()]);
                    for (int i = 0, j=1; i < vehicle.Route.Count-1; i++,j++)
                    {
                        Cost += GetDistance(CvrpGenetics.CvrProbelm.Locations[route[i]-1], CvrpGenetics.CvrProbelm.Locations[route[j]-1]);
                    }
                    // calc last ride back  to warehouse
                    Cost += GetDistance(CvrpGenetics.CvrProbelm.Locations[route.Last()-1], CvrpGenetics.CvrProbelm.Locations.First());                    
                }
            }
            Fitness = (uint)Math.Round(Cost);
        }

        private double GetDistance(Location location1, Location location2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(location1.X - location2.X),2) + Math.Pow(Math.Abs(location1.Y - location2.Y),2));
        }

        public List<int> GetRoutesPermutation()
        {
            List<int> routes = new List<int>();
            foreach (Vehicle vehicle in Vehicles)
            {
                routes =  new List<int>(routes.Concat(vehicle.Route));
            }
            return routes;
        }
    }
}