using System.Collections.Generic;
using SwarmIntel.Genetics_Sol;

namespace SwarmIntel.Tools
{
    public class Vehicle
    {
        public int Id;
        public int SupplyLeft; //current supply left on vechile to be delivered
        public List<int> Route; //route build of collection of location's id starting and ending with 0 (warehouse);
        public Vehicle(int id)
        {
            Id = id;
            SupplyLeft = ProblemData.CvrPro.Capacity;
            Route = new List<int>();
        }
        public Vehicle(Vehicle srcVehicle)
        {
            Id = srcVehicle.Id;
            SupplyLeft = srcVehicle.SupplyLeft;
            Route = new List<int>(srcVehicle.Route);
        }
    }
}
