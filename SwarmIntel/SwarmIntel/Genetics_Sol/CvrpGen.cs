using System;
using System.CodeDom;
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

        public CvrpGen(int numOfVehicles, List<Location> locations, Random rand)
        {
            Cost = 0;
            NumOfVehicles = numOfVehicles;
            Vehicles= new List<Vehicle>();
            for (int i = 1; i <= NumOfVehicles; i++)
            {
                Vehicles.Add(new Vehicle(i));
            }
            SupplyDemands(locations,rand);
        }

        public void SupplyDemands(List<Location> locations, Random rand)
        {
            List<Location> cities = new List<Location>();
            foreach (Location location in locations)
            {
                cities.Add(new Location(location));
            }
            cities.RemoveAt(0); //remove warehouse from cities
            List<Location> citiesSortedByDemmand =  cities.OrderByDescending(x => x.Demand).ToList();
            for (int i = 0; i < citiesSortedByDemmand.Count; i++)
            {
                bool delivered = false;
                while (delivered == false)
                {
                    int chosenVechileIndex = rand.Next() % Vehicles.Count;
                    var vehicle = Vehicles[chosenVechileIndex];
                    var city = citiesSortedByDemmand[i];
                    if (vehicle.SupplyLeft > 0 && city.Demand <= Vehicles[chosenVechileIndex].SupplyLeft)
                    {
                        vehicle.SupplyLeft -= (city.Demand - city.CurrentSupply);
                        city.CurrentSupply = city.Demand;
                        vehicle.Route.Add(citiesSortedByDemmand[i].Id);
                        delivered = true;
                    }
                }
            }           
        }

        public int VerifyRoutes()
        {
            List<Location> copyLocations = new List<Location>(CvrpGenetics.CvrPro.Locations);
            int shortage = 0;
            foreach (Vehicle vehicle in Vehicles)
            {
                int capacityLeft = CvrpGenetics.CvrPro.Capacity;
                int sum = 0;
                foreach (int cityId in vehicle.Route)
                {
                    capacityLeft -= copyLocations[cityId-1].Demand;
                    sum += copyLocations[cityId - 1].Demand;                   
                    if (sum > CvrpGenetics.CvrPro.Capacity)
                    {
                        var u = 900;
                    }
                }
                if (capacityLeft < 0)
                {
                    shortage += 10;
                }
            }

            return shortage;
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
                    Cost += GetDistance(CvrpGenetics.CvrPro.Locations.First(), CvrpGenetics.CvrPro.Locations[route.First()-1]);
                    for (int i = 0, j=1; i < vehicle.Route.Count-1; i++,j++)
                    {
                        Cost += GetDistance(CvrpGenetics.CvrPro.Locations[route[i]-1], CvrpGenetics.CvrPro.Locations[route[j]-1]);
                    }
                    // calc last ride back  to warehouse
                    Cost += GetDistance(CvrpGenetics.CvrPro.Locations[route.Last()-1], CvrpGenetics.CvrPro.Locations.First());                    
                }
            }
            Fitness = (uint)Math.Round(Cost);
            Fitness += (uint) VerifyRoutes();
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