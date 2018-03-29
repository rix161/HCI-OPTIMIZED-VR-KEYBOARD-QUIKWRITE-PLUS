using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardOptimized
{
    class Program
    {
        String[] getWordList(String Path) {
            string[] lines = System.IO.File.ReadAllLines(Path);
            return lines;
        }


        static void Main(string[] args)
        {
            String listPath = "F:\\Masters\\Sem2\\HCI\\Project\\docs\\wordList";
            String []wordFileName = {"usa_gen.txt","usa_short.txt","usa_medium.txt","usa_long.txt"};
            String[] probFileName = { "bigramProb.csv"};

            Optimizer o = new Optimizer();

            foreach(String fileName in wordFileName) {
                String filePath = listPath + "\\" + fileName;
                o.loadWordList(filePath);
            }
            o.loadBigramProb(listPath + "\\" + probFileName[0]);
            Dictionary<int, List<String>> bestKeyboard = o.doOptimization();

        }
    }
}
