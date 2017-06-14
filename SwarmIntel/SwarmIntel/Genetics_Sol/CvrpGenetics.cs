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
        public CvrpGenetics(CrossoverMethod crossMethod, SelectionMethod selectionMethod, MutationOperator mutationOperator) : base(crossMethod, selectionMethod)
        {
            _mutationOperator = mutationOperator;
            LocalOptSearchEnabled = true;
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
            switch (_mutationOperator)
            {
                case MutationOperator.Exchange:
                    var ipos2 = Rand.Next()% nQlist.Count;
                    nQlist = MutOpExchange(ipos1, ipos2, nQlist);                    
                    break;
                case MutationOperator.Displacement:
                    int numOfCities = ProblemData.CvrPro.Locations.Count - 1;
                    ipos2 = Rand.Next() % (numOfCities - ipos1) + ipos1;
                    nQlist = MutOpDisplacement(ipos1, ipos2, nQlist);
                    break;
            }
            for (int i = 0; i < ProblemData.CvrPro.NumOfVehicles; i++)
            {
                var count = member.Vehicles[i].Route.Count;
                member.Vehicles[i].Route.Clear();
                for (int q = 0; q < count; q++)
                {
                    member.Vehicles[i].Route.Add(nQlist.First());
                    nQlist.RemoveAt(0);
                }
            }
        }

        private List<int> MutOpExchange(int ipos1, int ipos2, List<int> nQlist)
        {
            int temp = nQlist[ipos1];
            nQlist[ipos1] = nQlist[ipos2];
            nQlist[ipos2] = temp;
            return nQlist;
        }

        private List<int> MutOpDisplacement(int ipos1, int ipos2, List<int> nQlist)
        {
            int numOfCities = ProblemData.CvrPro.Locations.Count - 1;
            int gapPos = Rand.Next() % (numOfCities - (ipos2 - ipos1));
            List<int> chosenPart = new List<int>();
            List<int> theRest = new List<int>(nQlist);
            for (int i = ipos1, count = 0; i < ipos2 + 1; i++, count++)
            {
                chosenPart.Add(nQlist[i]);
                theRest.RemoveAt(i - count);
            }
            for (int i = gapPos, j = 0; i < chosenPart.Count + gapPos; i++, j++)
            {
                theRest.Insert(i, chosenPart[j]);
            }
            return theRest;
        }

        protected override void mate_by_method(CvrpGen bufGen, CvrpGen gen1, CvrpGen gen2)
        {
            List<int> nQlist1 = gen1.GetRoutesPermutation();
            List<int> nQlist2 = gen2.GetRoutesPermutation();
            int ipos = Rand.Next() % nQlist1.Count;
            switch (CrosMethod)
            {
                case CrossoverMethod.Pmx:
                    nQlist2 = PmxCrossOver(nQlist1, nQlist2,ipos);                                     
                    break;
                case CrossoverMethod.Er:
                    nQlist2 = ErCrossOver(nQlist1, nQlist2);
                    break;
            }
            //rebuild the vehicles route's by the original route lenght
            for (int i = 0; i < ProblemData.CvrPro.NumOfVehicles; i++)
            {
                bufGen.Vehicles[i].Route.Clear();
                for (int q = 0; q < gen1.Vehicles[i].Route.Count; q++)
                {
                    bufGen.Vehicles[i].Route.Add(nQlist2.First());
                    nQlist2.RemoveAt(0);    
                }
            }
        }

        private List<int> ErCrossOver(List<int> nQlist1, List<int> nQlist2)
        {
            int numOfCities = ProblemData.CvrPro.Locations.Count-1;
            Dictionary<int, List<int>> neighborsDic1 = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> neighborsDic2 = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> neighborsDicMerged = new Dictionary<int, List<int>>();
            List<int> numbers = Enumerable.Range(2, numOfCities).ToList();
            for (int i = 0; i < numOfCities; i++)
            {
                List<int> neighbors1 = new List<int>();
                List<int> neighbors2 = new List<int>();
                int right = (i + 1) % numOfCities;
                int left = i - 1;
                if (left < 0) left = numOfCities - 1;
                neighbors1.Add(nQlist1[right]);
                neighbors1.Add(nQlist1[left]);
                neighbors2.Add(nQlist2[right]);
                neighbors2.Add(nQlist2[left]);
                neighborsDic1.Add(nQlist1[i], neighbors1);
                neighborsDic2.Add(nQlist2[i], neighbors2);
            }

            // join lists for all vals
            int p = 2;
            while (p <= numOfCities+1)
            {
                var list1 = neighborsDic1[p];
                var list2 = neighborsDic2[p];
                neighborsDicMerged.Add(p, list1.Union(list2).ToList());
                p++;
            }

            List<int> res = new List<int>();
            int link = nQlist1[0];
            res.Add(link);
            numbers.Remove(link);
            RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
            while (res.Count < numOfCities)
            {
                var neighbors = neighborsDicMerged[link];

                int minNeighbors = numOfCities;
                int tempIndex = 0;
                if (neighbors.Count > 0)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        if (neighborsDicMerged[neighbors[i]].Count < minNeighbors)
                        {
                            minNeighbors = neighborsDicMerged[neighbors[i]].Count;
                            tempIndex = i;
                        }
                    }
                    link = neighbors[tempIndex];
                    res.Add(link);
                    RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                    numbers.Remove(link);
                }
                else
                {
                    var index = Rand.Next() % numbers.Count;
                    link = numbers[index];
                    numbers.Remove(link);
                    res.Add(link);
                    RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                }
            }
            return nQlist2;
        }


        private List<int> PmxCrossOver(List<int> nQlist1, List<int> nQlist2, int ipos)
        {
            int val1 = nQlist1[ipos];
            int val2 = nQlist2[ipos];
            var index1 = nQlist1.IndexOf(val2);
            var index2 = nQlist2.IndexOf(val1);
            nQlist1[index1] = val1;
            nQlist2[index2] = val2;
            nQlist1[ipos] = val2;
            nQlist2[ipos] = val1;
            return nQlist2;
        }


        private void RemoveNeigbhorFromAllLists(Dictionary<int, List<int>> neighborsDicMerged, int target)
        {
            int numOfCities = ProblemData.CvrPro.Locations.Count;
            for (int i = 2; i <= numOfCities; i++)
            {
                var x = neighborsDicMerged[i];
                if (x.Contains(target))
                {
                    x.Remove(target);
                }
            }
        }

        protected override Tuple<string, uint> get_best_gen_details(CvrpGen gen)
        {
            string str = "";
            for (int i = 0; i < ProblemData.CvrPro.NumOfVehicles; i++)
            {
                str += i + ") ";
                foreach (int cityId in gen.Vehicles[i].Route)
                {
                    str += cityId + "=>";
                }
                str = str.Remove(str.Length - 2);
                str += "\n";
            }
            return new Tuple<string, uint>(str,gen.Fitness);
        }

        protected override CvrpGen get_new_gen()
        {
            return new CvrpGen(Rand,1);
        }

        protected override int calc_distance(CvrpGen gen1, CvrpGen gen2)
        {
            //Kendall tau distance
            int distance = 0;
            var per1 = gen1.GetRoutesPermutation();
            var per2 = gen2.GetRoutesPermutation();
            for (int i = 0; i < per1.Count; i++)
            {
                for (int j = i + 1; j < per2.Count; j++)
                {
                    if ((per1[i] < per1[j] && per2[i] > per2[j]) ||
                        (per1[i] > per1[j] && per2[i] < per2[j])) distance++;
                }
            }
            return distance;
        }

        public override void run_algorithm()
        {
            long totalTicks = 0;
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
                totalTicks += (long)ticks;

                print_result_details(Population[0], avg, stdDev, i);  // print the best one, average and std dev by iteration number                
                if (LocalOptSearchEnabled == true) search_local_optima(avg, stdDev, i);
                stopWatch.Restart(); // restart timers for next iteration
                if ((Population)[0].Fitness <= ProblemData.CvrPro.Opt)
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
        }        
    }
}