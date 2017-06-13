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
        public CvrpGen(CvrpGen source) //copy ctor
        {
            Cost = source.Cost;
            Vehicles = new List<Vehicle>();
            for (int i = 0; i < ProblemData.CvrPro.NumOfVehicles; i++)
            {
                Vehicles.Add(new Vehicle(source.Vehicles[i]));
            }
        }
        public CvrpGen(Random rand,int suppyPreference = 1)
        {
            Cost = 0;
            Vehicles= new List<Vehicle>();
            for (int i = 1; i <= ProblemData.CvrPro.NumOfVehicles; i++)
            {
                Vehicles.Add(new Vehicle(i));
            }
            if (suppyPreference==1) SupplyDemandsBySize(rand);
            else if (suppyPreference==2) SupplyDemandsByDistance(rand);
        }
        private void SupplyDemandsByDistance(Random rand)
        {
            List<Location> cities = new List<Location>();
            foreach (Location loc in ProblemData.CvrPro.Locations)
                cities.Add(new Location(loc));
            //sort cities from warehouse to another city an so on till last city
            List<Location> citiesSortedByDistances = new List<Location>(); 
            Location nearestCity = new Location(ProblemData.CvrPro.Locations[0]); //warehouse
            citiesSortedByDistances.Add(nearestCity);
            cities.RemoveAt(0);  
            for (int i = 0; i < ProblemData.CvrPro.Locations.Count-1; i++)
            {
                nearestCity = Location.GetNearestCity(nearestCity, cities);
                citiesSortedByDistances.Add(nearestCity);
            }
            foreach (Vehicle vehicle in Vehicles)
            {
                for (int i = 0; i < citiesSortedByDistances.Count; i++)
                {
                    var city = citiesSortedByDistances[i];
                    if (city.Demand <= vehicle.SupplyLeft)
                    {
                        vehicle.SupplyLeft -= (city.Demand - city.CurrentSupply);
                        city.CurrentSupply = city.Demand;
                        vehicle.Route.Add(city.Id);
                        citiesSortedByDistances.Remove(city);
                    }                        
                }
            }

            if (citiesSortedByDistances.Count > 0)
            {
                for (int i = 0; i < citiesSortedByDistances.Count; i++)
                {
                    bool delivered = false;
                    while (delivered == false)
                    {
                        int chosenVechileIndex = rand.Next() % Vehicles.Count;
                        var vehicle = Vehicles[chosenVechileIndex];
                        var city = citiesSortedByDistances[i];
                        if (vehicle.SupplyLeft > 0 && city.Demand <=
                                Vehicles[chosenVechileIndex].SupplyLeft)
                        {
                            vehicle.SupplyLeft -= (city.Demand - city.CurrentSupply);
                            city.CurrentSupply = city.Demand;
                            vehicle.Route.Add(citiesSortedByDistances[i].Id);
                            delivered = true;
                        }
                    }
                }
            }                            
        }

        
        
        public void SupplyDemandsBySize(Random rand)
        {
            List<Location> cities = new List<Location>();
            for (int i = 1; i < ProblemData.CvrPro.Locations.Count; i++) //i=1 to skip warehouse
            {
                cities.Add(new Location(ProblemData.CvrPro.Locations[i]));
                
            }           
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

        public int VerifyRoutesAndSupply()
        {
            List<Location> copyLocations = new List<Location>(ProblemData.CvrPro.Locations);
            int shortage = 0;
            int numOfVisitedCities = 0;
            foreach (Vehicle vehicle in Vehicles)
            {
                vehicle.SupplyLeft = ProblemData.CvrPro.Capacity;
                foreach (int cityId in vehicle.Route)
                {
                    numOfVisitedCities++;
                    vehicle.SupplyLeft -= copyLocations[cityId-1].Demand;
                }
                numOfVisitedCities-=2;
                if (vehicle.SupplyLeft < 0)
                {
                    shortage += 10;
                }
            }
            if (numOfVisitedCities< ProblemData.CvrPro.Locations.Count - 1)
                shortage += 100*(ProblemData.CvrPro.Locations.Count - 1 - numOfVisitedCities);
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
                    Cost += Location.GetDistance(ProblemData.CvrPro.Locations.First(),
                                                  ProblemData.CvrPro.Locations[route.First()-1]);
                    for (int i = 0, j=1; i < vehicle.Route.Count-1; i++,j++)
                    {
                        Cost += Location.GetDistance(ProblemData.CvrPro.Locations[route[i]-1],
                                                      ProblemData.CvrPro.Locations[route[j]-1]);
                    }
                    // calc last ride back  to warehouse
                    Cost += Location.GetDistance(ProblemData.CvrPro.Locations[route.Last()-1],
                                                  ProblemData.CvrPro.Locations.First());                    
                }
            }
            Fitness = (uint)Math.Round(Cost);
            Fitness += (uint) VerifyRoutesAndSupply();
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