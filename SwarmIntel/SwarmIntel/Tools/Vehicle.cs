using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SupplyLeft = CvrpGenetics.CvrPro.Capacity;
            Route = new List<int>();
        }
    }
}
