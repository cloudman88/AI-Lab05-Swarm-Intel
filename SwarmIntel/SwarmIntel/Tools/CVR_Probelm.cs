using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SwarmIntel.Tools
{    
    enum CvrProblemsFiles
    {
        En22k4,
        En33k4,
        En51k5,
        En76k8,
        En76k10,
        En101k8,
        En101k14
    }
    // ReSharper disable once InconsistentNaming
    public class CVR_Probelm
    {
        public int Dim;
        public int Capacity;
        public List<Location> Locations;

        public CVR_Probelm(int probelmNum)
        {
            var cvrProblems = Enum.GetValues(typeof(CvrProblemsFiles)).Cast<CvrProblemsFiles>()
                                       .Select(x => x.ToString()).ToArray();
            string relavtiePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\CVR_Problems\";
            string filePath = relavtiePath + cvrProblems[probelmNum-1] +".txt";
            List<int[]> locationsInput = new List<int[]>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                //skip the first 3 lines
                for (int i = 0; i < 3; i++)
                {
                    sr.ReadLine();
                }
                //read dimention
                if ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] {"\t", " ", ":"}, StringSplitOptions.RemoveEmptyEntries);
                    Dim = Int32.Parse(data[1]);
                }
                //skip the 5th line
                sr.ReadLine();
                //read capacity
                if ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] {"\t", " ", ":"}, StringSplitOptions.RemoveEmptyEntries);
                    Capacity = Int32.Parse(data[1]);
                }
                //skip the 7th line
                sr.ReadLine();
                //read locations id and coordinates
                while ((line = sr.ReadLine()) != null && !line.Equals("DEMAND_SECTION"))
                {
                    int[] locInput = new int[4];
                    string[] data = line.Split(new string[] {"\t", " "}, StringSplitOptions.RemoveEmptyEntries);
                    int[] numbers = Array.ConvertAll(data, int.Parse);
                    locInput[0] = numbers[0];
                    locInput[1] = numbers[1];
                    locInput[2] = numbers[2];
                    locationsInput.Add(locInput);
                }
                //read locations demands
                while ((line = sr.ReadLine()) != null && !line.Equals("DEPOT_SECTION"))
                {
                    string[] data = line.Split(new string[] {"\t", " "}, StringSplitOptions.RemoveEmptyEntries);
                    int[] numbers = Array.ConvertAll(data, int.Parse);
                    locationsInput[numbers[0]-1][3] = numbers[1];
                }

                //create locations list
                Locations = new List<Location>();
                foreach (var loc in locationsInput)
                {
                    Locations.Add(new Location
                    {
                        Id = loc[0],
                        X = loc[1],
                        Y = loc[2],
                        Demand = loc[3]
                    });
                }
            }
        }
    }

    public struct Location
    {
        public int Id;
        public int X;
        public int Y;
        public int Demand;
    }

    public struct Vehicle
    {
        public int Id;
        public int Total; //current demand delivered
        public List<int> Route; //route build of collection of location's id starting and ending with 0 (warehouse);
    }
}