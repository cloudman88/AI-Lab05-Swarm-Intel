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
            Manager man = new Manager();
            do
            {
                ProblemData pdData = new ProblemData(1);
                CvrpGenetics cvrpGenetics = new CvrpGenetics(CrossoverMethod.Er,SelectionMethod.Tournament,MutationOperator.Displacement);
                cvrpGenetics.init_population();
                cvrpGenetics.run_algorithm();
                //man.Run();
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
