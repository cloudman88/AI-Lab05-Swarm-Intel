using System;
using System.Collections.Generic;
using System.Linq;
using SwarmIntel.ACO_Sol;
using SwarmIntel.GeneticsAlgorithm;
using SwarmIntel.Genetics_Sol;
using SwarmIntel.Local_Search_Sol;
using SwarmIntel.Tools;

namespace SwarmIntel
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                ProblemData pData = new ProblemData(1);
                CVRP_AOC cvrpAoc = new CVRP_AOC();
                cvrpAoc.Init();
                cvrpAoc.run_algorithm();

                //CvrpGenetics cvrpGenetics = new CvrpGenetics(CrossoverMethod.Pmx, SelectionMethod.Tournament, MutationOperator.Displacement);
                //cvrpGenetics.init_population();
                //cvrpGenetics.run_algorithm();

                //CvrpLocalSearch cvrpLocalSearch = new CvrpLocalSearch();
                //cvrpLocalSearch.Init();
                //cvrpLocalSearch.run_algorithm();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
