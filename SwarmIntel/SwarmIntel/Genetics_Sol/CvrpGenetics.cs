using SwarmIntel.GeneticsAlgorithm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SwarmIntel.Tools;

namespace SwarmIntel.Genetics_Sol
{
    class CvrpGenetics : GeneticsAlgorithms <CvrpGen>
    {
        private readonly MutationOperator _mutationOperator;
        public static CvrProbelm CvrPro;
        public CvrpGenetics(CrossoverMethod crossMethod, SelectionMethod selectionMethod, MutationOperator mutationOperator) : base(crossMethod, selectionMethod)
        {
            _mutationOperator = mutationOperator;
            CvrPro = new CvrProbelm(1);
            LocalOptSearchEnabled = false;
        }

        public override void init_population()
        {
            for (int i = 0; i < GaPopSize; i++)
            {
                Population.Add(new CvrpGen(Rand));
                Buffer.Add(new CvrpGen(Rand));
            }
        }

        protected override void calc_fitness()
        {
            for (int i = 0; i < GaPopSize; i++)
            {
                Population[i].CalcCost();
            }
        }

        protected override void Mutate(CvrpGen member)
        {
            List<int> nQlist = member.GetRoutesPermutation();
            int ipos1 = Rand.Next()%nQlist.Count;
            int ipos2;
            switch (_mutationOperator)
            {
                case MutationOperator.Exchange:
                    ipos2 = Rand.Next()% nQlist.Count;
                    nQlist = MutOpExchange(ipos1, ipos2, nQlist);
                    for (int i = 0; i < CvrPro.NumOfVehicles; i++)
                    {
                        var count = member.Vehicles[i].Route.Count;
                        member.Vehicles[i].Route.Clear();
                        for (int q = 0; q < count; q++)
                        {
                            member.Vehicles[i].Route.Add(nQlist.First());
                            nQlist.RemoveAt(0);
                        }
                    }
                    break;
            }
        }

        private List<int> MutOpExchange(int ipos1, int ipos2, List<int> nQlist)
        {
            int temp = nQlist[ipos1];
            nQlist[ipos1] = nQlist[ipos2];
            nQlist[ipos2] = temp;
            return nQlist;
        }

        protected override void mate_by_method(CvrpGen bufGen, CvrpGen gen1, CvrpGen gen2)
        {
            List<int> nQlist1 = gen1.GetRoutesPermutation();
            List<int> nQlist2 = gen2.GetRoutesPermutation();
            int ipos = Rand.Next() % nQlist1.Count;
            switch (CrosMethod)
            {
                case CrossoverMethod.Pmx:
                    int val1 = nQlist1[ipos];
                    int val2 = nQlist2[ipos];
                    var index1 = nQlist1.IndexOf(val2);
                    var index2 = nQlist2.IndexOf(val1);
                    nQlist1[index1] = val1;
                    nQlist2[index2] = val2;
                    nQlist1[ipos] = val2;
                    nQlist2[ipos] = val1;
                    for (int i = 0; i < CvrPro.NumOfVehicles; i++)
                    {
                        bufGen.Vehicles[i].Route.Clear();
                        for (int q = 0; q < gen1.Vehicles[i].Route.Count; q++)
                        {
                            bufGen.Vehicles[i].Route.Add(nQlist2.First());
                            nQlist2.RemoveAt(0);    
                        }
                    }
                    break;
                case CrossoverMethod.Cx:
                    List<int> cycle = new List<int>();
                    List<int> result = Enumerable.Repeat(-1, ProblemData.CvrPro.Locations.Count-1).ToList();
                    int k = 1;
                    var start = nQlist1[0];
                    cycle.Add(start);
                    var end = nQlist2[0];
                    if (start != end)
                    {
                        cycle.Add(nQlist2[0]);
                        while (k < ProblemData.CvrPro.Locations.Count-1)
                        {
                            var endIndex = nQlist1.IndexOf(end);
                            end = nQlist2[endIndex];
                            if (start == end) break;
                            cycle.Add(end);
                            k++;
                        }
                    }

                    foreach (var val in cycle)
                    {
                        var index = nQlist1.IndexOf(val);
                        result[index] = val;
                    }
                    k = 0;
                    while (k < ProblemData.CvrPro.Locations.Count-1)
                    {
                        if (result[k] == -1)
                        {
                            result[k] = nQlist2[k];
                        }
                        k++;
                    }
                    for (int i = 0; i < CvrPro.NumOfVehicles; i++)
                    {
                        bufGen.Vehicles[i].Route.Clear();
                        for (int q = 0; q < gen1.Vehicles[i].Route.Count; q++)
                        {
                            bufGen.Vehicles[i].Route.Add(nQlist2.First());
                            nQlist2.RemoveAt(0);
                        }
                    }
                    break;                  
            }
        }

        protected override Tuple<string, uint> get_best_gen_details(CvrpGen gen)
        {
            string str = "";
            for (int i = 0; i < CvrPro.NumOfVehicles; i++)
            {
                str += i + ") ";
                foreach (int cityId in gen.Vehicles[i].Route)
                {
                    str += cityId + "=>";
                }
                str = str.Remove(str.Length - 2);
                str += " ";
            }
            return new Tuple<string, uint>(str,gen.Fitness);
        }

        protected override CvrpGen get_new_gen()
        {
            throw new NotImplementedException();
        }

        protected override int calc_distance(CvrpGen gen1, CvrpGen gen2)
        {
            throw new NotImplementedException();
        }

        public override void run_algorithm()
        {
            long totalTicks = 0;
            long totalElasped = 0;
            int totalIteration = -1;
            Stopwatch stopWatch = new Stopwatch(); //stopwatch is used for both clock ticks and elasped time measuring
            stopWatch.Start();
            for (int i = 0; i < GaMaxiter; i++)
            {
                calc_fitness();      // calculate fitness
                sort_by_fitness();   // sort them
                var avg = calc_avg(); // calc avg
                var stdDev = calc_std_dev(avg); //calc std dev

                //calculate time differences                
                stopWatch.Stop();
                double ticks = (stopWatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000;
                double elasped = (stopWatch.Elapsed.Ticks / (double)Stopwatch.Frequency) * 1000;
                totalElasped += (long)elasped;
                totalTicks += (long)ticks;

                print_result_details(Population[0], avg, stdDev, i);  // print the best one, average and std dev by iteration number                
                if (LocalOptSearchEnabled == true) search_local_optima(avg, stdDev, i);

                stopWatch.Restart(); // restart timers for next iteration
                if ((Population)[0].Fitness <= CvrPro.Opt)
                {
                    totalIteration = i + 1; // save number of iteration            
                    break;
                }
                Mate();     // mate the population together
                swap_population_with_buffer();       // swap buffers
            }
            if (totalIteration == GaMaxiter)
            {
                Console.WriteLine("Failed to find solution in " + totalIteration + " iterations.");
            }
            else
            {
                Console.WriteLine("Iterations: " + totalIteration);
            }
            Console.WriteLine("\nTimig in milliseconds:");
            Console.WriteLine("Total Ticks " + totalTicks);
            Console.WriteLine("Total Elasped " + totalElasped);
            Console.WriteLine("Elaspeds - Ticks = " + (totalElasped - totalTicks) + "\n");
        }        
    }
}