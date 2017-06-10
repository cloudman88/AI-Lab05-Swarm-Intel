using System;
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
                //CvrpGenetics cvrpGenetics = new CvrpGenetics(CrossoverMethod.Pmx, SelectionMethod.Truncation,MutationOperator.Exchange);
                //cvrpGenetics.init_population();
                //cvrpGenetics.run_algorithm();        
                ProblemData pData = new ProblemData(1);
                CvrpLocalSearch cvrpLocalSearch = new CvrpLocalSearch(1);
                cvrpLocalSearch.Init();
                cvrpLocalSearch.run_algorithm();
           } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
