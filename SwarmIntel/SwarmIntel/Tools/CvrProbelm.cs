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
    public class CvrProbelm
    {
        public int Capacity;
        public int Opt;
        public int NumOfVehicles;
        public List<Location> Locations;

        public CvrProbelm(int probelmNum)
        {
            var cvrProblems = Enum.GetValues(typeof(CvrProblemsFiles)).Cast<CvrProblemsFiles>()
                                       .Select(x => x.ToString()).ToArray();
            string relavtiePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\CVR_Problems\";
            string filePath = relavtiePath + cvrProblems[probelmNum-1] +".txt";
            List<int[]> locationsInput = new List<int[]>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                //skip the first line
                sr.ReadLine();
                //read num of cars, opt value
                if ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new string[] {"\t", " ", ":"}, StringSplitOptions.RemoveEmptyEntries);
                    NumOfVehicles = Int32.Parse(data[8].Substring(0, data[8].Length-1));
                    Opt = Int32.Parse(data[11].Substring(0, data[11].Length - 1));
                }
                //skip the first lines 3-5
                for (int i = 0; i < 3; i++)
                {
                    sr.ReadLine();
                }
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
                    Locations.Add(new Location(loc[0],loc[1],loc[2],loc[3]));
                }
            }
        }
    }    
}