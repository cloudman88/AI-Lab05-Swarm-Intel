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
        public double Cost;

        public CvrpGen(Random rand)
        {
            Cost = 0;
            Vehicles= new List<Vehicle>();
            for (int i = 1; i <= ProblemData.CvrPro.NumOfVehicles; i++)
            {
                Vehicles.Add(new Vehicle(i));
            }
            SupplyDemands(rand);
        }

        public CvrpGen(CvrpGen source)
        {
            Cost = source.Cost;
            Vehicles = new List<Vehicle>();
            for (int i = 0; i < ProblemData.CvrPro.NumOfVehicles; i++)
            {                
                Vehicles.Add(new Vehicle(source.Vehicles[i]));
            }
        }

        public void SupplyDemands(Random rand)
        {
            List<Location> cities = new List<Location>();
            foreach (Location location in ProblemData.CvrPro.Locations)
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
            List<Location> copyLocations = new List<Location>(ProblemData.CvrPro.Locations);
            int shortage = 0;
            foreach (Vehicle vehicle in Vehicles)
            {
                int capacityLeft = ProblemData.CvrPro.Capacity;
                int sum = 0;
                foreach (int cityId in vehicle.Route)
                {
                    capacityLeft -= copyLocations[cityId-1].Demand;
                    sum += copyLocations[cityId - 1].Demand;                   
                    if (sum > ProblemData.CvrPro.Capacity)
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
                    Cost += Location.GetDistance(ProblemData.CvrPro.Locations.First(), ProblemData.CvrPro.Locations[route.First()-1]);
                    for (int i = 0, j=1; i < vehicle.Route.Count-1; i++,j++)
                    {
                        Cost += Location.GetDistance(ProblemData.CvrPro.Locations[route[i]-1], ProblemData.CvrPro.Locations[route[j]-1]);
                    }
                    // calc last ride back  to warehouse
                    Cost += Location.GetDistance(ProblemData.CvrPro.Locations[route.Last()-1], ProblemData.CvrPro.Locations.First());                    
                }
            }
            Fitness = (uint)Math.Round(Cost);
            Fitness += (uint) VerifyRoutes();
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