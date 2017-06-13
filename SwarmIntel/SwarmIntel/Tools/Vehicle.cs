using System.Collections.Generic;

namespace SwarmIntel.Tools
{
    public class Vehicle
    {
        public int Id;
        //current supply left on vechile to be delivered:
        public int SupplyLeft;
        //route is collection of location's id starting and ending with 0 (warehouse):
        public List<int> Route; 
        public Vehicle(int id)
        {
            Id = id;
            SupplyLeft = ProblemData.CvrPro.Capacity;
            Route = new List<int>();
        }
        public Vehicle(Vehicle srcVehicle) //copy ctor
        {
            Id = srcVehicle.Id;
            SupplyLeft = srcVehicle.SupplyLeft;
            Route = new List<int>(srcVehicle.Route);
        }
    }
}