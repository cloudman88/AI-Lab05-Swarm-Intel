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
        public static CvrProbelm CvrProbelm;
        public CvrpGenetics(CrossoverMethod crossMethod, SelectionMethod selectionMethod, MutationOperator mutationOperator) : base(crossMethod, selectionMethod)
        {
            _mutationOperator = mutationOperator;
            CvrProbelm = new CvrProbelm(1);
            LocalOptSearchEnabled = false;
        }

        public override void init_population()
        {
            for (int i = 0; i < GaPopSize; i++)
            {
                Population.Add(new CvrpGen(CvrProbelm.NumOfVehicles, CvrProbelm.Locations));
                Buffer.Add(new CvrpGen(CvrProbelm.NumOfVehicles, CvrProbelm.Locations));
            }
        }

        protected override void calc_fitness()
        {
            for (int i = 0; i < GaPopSize; i++)
            {
                //Population[i].SupplyDemands(CvrProbelm.Locations);
                Population[i].CalcCost();
            }
        }

        protected override void Mutate(CvrpGen member)
        {
            List<int> nQlist = member.GetRoutesPermutation();
            int ipos1 = Rand.Next()%nQlist.Count;
            int ipos2;
            checkcars();
            switch (_mutationOperator)
            {
                case MutationOperator.Exchange:
                    ipos2 = Rand.Next()% nQlist.Count;
                    nQlist = MutOpExchange(ipos1, ipos2, nQlist);
                    checkcars();
                    for (int i = 0; i < CvrProbelm.NumOfVehicles; i++)
                    {
                        var count = member.Vehicles[i].Route.Count;
                        for (int q = 0; q < count; q++)
                        {
                            member.Vehicles[i].Route.Add(nQlist.First());
                            nQlist.RemoveAt(0);
                        }
                        checkcars();
                    }
                    checkcars();
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
            //int qpos2 = Rand.Next() % (volsSize - qpos1) + qpos1;
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
                    for (int i = 0; i < CvrProbelm.NumOfVehicles; i++)
                    {
                        bufGen.Vehicles[i].Route.Clear();
                        for (int q = 0; q < gen2.Vehicles[i].Route.Count; q++)
                        {
                            bufGen.Vehicles[i].Route.Add(nQlist2.First());
                            nQlist2.RemoveAt(0);    
                                                                              
                        }
                        if (bufGen.Vehicles[i].Route.Count > 22)
                        {
                            var ipd = 9;
                        }
                    }
                    break;
                //case CrossoverMethod.OX:
                //    List<int> temp = Enumerable.Repeat(-1, volsSize).ToList();
                //    List<int> numbers = Enumerable.Range(0, volsSize).ToList();

                //    for (int i = 0; i < volsSize / 2; i++)
                //    {
                //        temp[numbers[ipos]] = nQlist1[numbers[ipos]];
                //        numbers.RemoveAt(ipos);
                //        ipos = Rand.Next() % numbers.Count;
                //    }
                //    int j = 0;
                //    foreach (var val in nQlist2)
                //    {
                //        if (!temp.Contains(val))
                //        {
                //            while (j < temp.Count && temp[j] != -1)
                //            {
                //                j++;
                //            }
                //            if (j != temp.Count) temp[j] = val;
                //        }
                //    }
                //    bufGen.Volumes = temp;
                //    break;
                //case CrossoverMethod.CX:
                //    List<int> cycle = new List<int>();
                //    List<int> result = Enumerable.Repeat(-1, volsSize).ToList();
                //    int k = 1;
                //    var start = nQlist1[0];
                //    cycle.Add(start);
                //    var end = nQlist2[0];
                //    if (start != end)
                //    {
                //        cycle.Add(nQlist2[0]);
                //        while (k < volsSize)
                //        {
                //            var endIndex = nQlist1.IndexOf(end);
                //            end = nQlist2[endIndex];
                //            if (start == end) break;
                //            cycle.Add(end);
                //            k++;
                //        }
                //    }

                //    foreach (var val in cycle)
                //    {
                //        var index = nQlist1.IndexOf(val);
                //        result[index] = val;
                //    }
                //    k = 0;
                //    while (k < volsSize)
                //    {
                //        if (result[k] == -1)
                //        {
                //            result[k] = nQlist2[k];
                //        }
                //        k++;
                //    }
                //    bufGen.Volumes = result;
                //    break;
                //case CrossoverMethod.ER:
                //    Dictionary<int, List<int>> neighborsDic1 = new Dictionary<int, List<int>>();
                //    Dictionary<int, List<int>> neighborsDic2 = new Dictionary<int, List<int>>();
                //    Dictionary<int, List<int>> neighborsDicMerged = new Dictionary<int, List<int>>();

                //    for (int i = 0; i < volsSize; i++)
                //    {
                //        List<int> neighbors1 = new List<int>();
                //        List<int> neighbors2 = new List<int>();
                //        int right = (i + 1) % volsSize;
                //        int left = i - 1;
                //        if (left < 0) left = volsSize - 1;
                //        neighbors1.Add(nQlist1[right]);
                //        neighbors1.Add(nQlist1[left]);
                //        neighbors2.Add(nQlist2[right]);
                //        neighbors2.Add(nQlist2[left]);
                //        neighborsDic1.Add(nQlist1[i], neighbors1);
                //        neighborsDic2.Add(nQlist2[i], neighbors2);
                //    }

                //    // join lists for all vals
                //    int p = 0;
                //    while (p < volsSize)
                //    {
                //        var list1 = neighborsDic1[p];
                //        var list2 = neighborsDic2[p];
                //        neighborsDicMerged.Add(p, list1.Union(list2).ToList());
                //        p++;
                //    }

                //    List<int> res = new List<int>();
                //    int link = nQlist1[0];
                //    res.Add(link);

                //    numbers = Enumerable.Range(0, volsSize).ToList();
                //    numbers.Remove(link);
                //    RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                //    while (res.Count < volsSize)
                //    {
                //        var neighbors = neighborsDicMerged[link];

                //        int minNeighbors = volsSize;
                //        int tempIndex = 0;
                //        if (neighbors.Count > 0)
                //        {
                //            for (int i = 0; i < neighbors.Count; i++)
                //            {
                //                if (neighborsDicMerged[neighbors[i]].Count < minNeighbors)
                //                {
                //                    minNeighbors = neighborsDicMerged[neighbors[i]].Count;
                //                    tempIndex = i;
                //                }
                //            }
                //            link = neighbors[tempIndex];
                //            res.Add(link);
                //            RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                //            numbers.Remove(link);
                //        }
                //        else
                //        {
                //            var index = Rand.Next() % numbers.Count;
                //            link = numbers[index];
                //            numbers.Remove(link);
                //            res.Add(link);
                //            RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                //        }
                //    }
                //    bufGen.Volumes = res;

                //    break;
            }
        }

        protected override Tuple<string, uint> get_best_gen_details(CvrpGen gen)
        {
            string str = "";
            for (int i = 0; i < CvrProbelm.NumOfVehicles; i++)
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
                checkcars();
                calc_fitness();      // calculate fitness
                checkcars();
                sort_by_fitness();   // sort them
                checkcars();
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
                if ((Population)[0].Fitness == CvrProbelm.Opt)
                {
                    totalIteration = i + 1; // save number of iteration                                                           
                    break;
                }
                Mate();     // mate the population together
                checkcars();
                swap_population_with_buffer();       // swap buffers
                checkcars();
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

        private void checkcars()
        {
            foreach (CvrpGen cvrpGen in Population)
            {
                var x = cvrpGen.GetRoutesPermutation();
                if (x.Count > 22)
                {
                    var ko = 90;
                }
                foreach (Vehicle vehicle in cvrpGen.Vehicles)
                {
                    if (vehicle.Route.Count > 22)
                    {
                        var dfd = 9;
                    }
                }
            }
        }
    }
}