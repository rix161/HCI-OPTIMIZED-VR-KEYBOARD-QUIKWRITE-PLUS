using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KeyboardOptimized
{
    class Optimizer{
        private String[] wordList;
        private double [,]bigramProdTable = new double[26,26];
        KeyboardManager KeyMaster;

        private String[] alphabetList = { "A","B","C","D","E",
                                      "F","G","H","I","J",
                                      "K","L","M","N","O",
                                      "P","Q","R","S","T",
                                      "U","V","W","X","Y",
                                      "Z" };


        public Optimizer() {
            KeyMaster = new KeyboardManager();

        }

        public void loadWordList(String listPath){
                this.wordList = System.IO.File.ReadAllLines(listPath);
           

        }

        public void loadBigramProb(String listPath) {
            String[] allLines = System.IO.File.ReadAllLines(listPath);
            foreach (String line in allLines) {
                String []subStr = line.Split(',');
                Char[] charSubStr = subStr[0].ToCharArray();
                int val1 = (int)charSubStr[0] - (int)'A';
                int val2 = (int)charSubStr[1] - (int)'A';
                bigramProdTable[val1, val2] = Convert.ToDouble(subStr[1]);
            }
            
        }

        private double acceptanceProbability(double currDist, double newDist,double temp) {
            if (newDist < currDist)
            {
                return 1.1;
            }
           
            return Math.Exp((currDist - newDist) / temp);
        }


        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public Dictionary<int, List<String>> doOptimization()
        {
            double temp = 100000;
            double coolingRate = 0.000003;
            double bestDistance = computeCost();
            Dictionary<int, List<String>> bestResult = KeyMaster.getKeyboard();
            Random randGen = new Random();
            double currDistance = bestDistance;

            while (temp > 1)
            {
                int charI1 = randGen.Next(0, 25);
                int charI2 = randGen.Next(0, 25);
                KeyMaster.swapElements(alphabetList[charI1], alphabetList[charI2]);
                double newDistance = computeCost();

                if (acceptanceProbability(currDistance, newDistance, temp) >= randGen.Next(0,1)){
                    currDistance = newDistance;
                    if(currDistance < bestDistance)
                    {
                        bestDistance = currDistance;
                        bestResult = DeepClone(KeyMaster.getKeyboard());
                    }
                }
                else
                {
                    KeyMaster.swapElements(alphabetList[charI2], alphabetList[charI1]);
                }
                temp *= 1 - coolingRate;
            }

            Console.WriteLine("BestResult:" + bestResult);
            return bestResult;
        }


        private double computeCost() {


           double totalDistance = 0.0f;

            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    totalDistance += bigramProdTable[i, j] * Math.Log(KeyMaster.getDistance(alphabetList[i], alphabetList[j]) + 1) / Math.Log(2);
                    KeyMaster.resetCursor();
                }
            }
            //Console.WriteLine("totalDistance:" + totalDistance);
            return totalDistance;
        }

    }


}
