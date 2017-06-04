using System;
using SwarmIntel.GeneticsAlgorithm;
using SwarmIntel.Genetics_Sol;
using SwarmIntel.Tools;

namespace SwarmIntel
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                CvrpGenetics cvrpGenetics = new CvrpGenetics(CrossoverMethod.Pmx, SelectionMethod.Tournament,MutationOperator.Exchange);
                cvrpGenetics.init_population();
                cvrpGenetics.run_algorithm();                
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
