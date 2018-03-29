using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardOptimized
{
    class KeyboardManager{

        private Dictionary<int, List<String>> mCKeyboard = new Dictionary<int, List<String>>();
        private Dictionary<int, Tuple<float, float>> mPosToLoc = new Dictionary<int, Tuple<float, float>>();
        private Tuple<float, float> cPos = new Tuple<float, float>(0, 0);

        //HARDCODED VALUES
        //GRID NUMBERING STARTS WITH 0
        private float NO_CROSSING_ODD = 0.5f;
        private float NO_CROSSING_EVEN = 0.25f;
        private float SINGLE_CROSSING = 1.059f;
        private float DOUBLE_CROSSING = 2.159f;
        private int[] SINGLE_CROSSING_POS = { 0, 4 };
        private int[] DOUBLE_CROSSING_POS = { 1, 3 };
        private int MIDDLE_POS = 2;
        private int MAX_GRID = 7;
        private int MIN_GRID = 0;

        private Dictionary<Tuple<Tuple<float, float>, Tuple<float, float>>, double> euclideanDistCach = new Dictionary<Tuple<Tuple<float, float>, Tuple<float, float>>, double>();

        private Dictionary<int, List<String>> getDefaultLayout(){
            Dictionary<int, List<String>> keyboard = new Dictionary<int, List<String>>();
            //dist 1.8275704778766939
            keyboard[0] = new List<string> { "K", "S", "A", "M", "Q" };
            keyboard[1] = new List<string> { "H", "", "E", "","C"};
            keyboard[2] = new List<string> { "V", "W", "O", "G", "Z" };
            keyboard[3] = new List<string> { "", " ", ""};
            keyboard[4] = new List<string> { "B", "D", "I", "R", "J" };
            keyboard[5] = new List<string> { "Y","", "T", "","U"};
            keyboard[6] = new List<string> { "X", "L", "N", "F", "P" };
            keyboard[7] = new List<string> { "", "$", ""};

            return keyboard;
        }


        private Dictionary<int, List<String>> getOptimizedLayout()
        {
            Dictionary<int, List<String>> keyboard = new Dictionary<int, List<String>>();
            //Dist:0.95278605926341853
            keyboard[0] = new List<string> { "S", "P", "E", "V", "T" };
            keyboard[1] = new List<string> { "R", "", "U", "", "K" };
            keyboard[2] = new List<string> { "H", "X", "O", "G", "Z" };
            keyboard[3] = new List<string> { "", " ", "" };
            keyboard[4] = new List<string> { "C", "Q", "N", "J", "D" };
            keyboard[5] = new List<string> { "Y", "", "A", "", "L" };
            keyboard[6] = new List<string> { "M", "F", "I", "B", "W" };
            keyboard[7] = new List<string> { "", "$", "" };

            return keyboard;
        }

        public Dictionary<int, List<String>> getKeyboard() {
            return this.mCKeyboard;
        }

        private Dictionary<int, Tuple<float, float>> getDefaultLocation(){
            Dictionary<int, Tuple<float , float>> locs = new Dictionary<int, Tuple<float, float>>();

            locs[0] = new Tuple<float, float>(-0.25f,+0.25f);
            locs[1] = new Tuple<float, float>(+0.00f,+0.25f);
            locs[2] = new Tuple<float, float>(+0.25f,+0.25f);
            locs[3] = new Tuple<float, float>(-0.25f,+0.00f);
            locs[4] = new Tuple<float, float>(+0.25f,+0.00f);
            locs[5] = new Tuple<float, float>(-0.25f,-0.25f);
            locs[6] = new Tuple<float, float>(+0.00f,-0.25f);
            locs[7] = new Tuple<float, float>(+0.25f,-0.25f);
            locs[8] = new Tuple<float, float>(+0.00f,+0.00f);

            return locs;
        }
        public KeyboardManager() {
            mCKeyboard = getDefaultLayout();
            mPosToLoc  = getDefaultLocation();
        }

        private double computeEuclidianCost(Tuple<float, float> pos1, Tuple<float, float> pos2) {
            float eDistance = 0.0f;
            float dx = pos2.Item1 - pos1.Item1;
            float dy = pos2.Item2 - pos1.Item2;
            eDistance = dx * dx + dy * dy;
            return Math.Sqrt(eDistance);
        }

        private float computeCrossingCost(Tuple<int, int> charLoc, Tuple<float, float> finalPos) {

            float distance = 0.0f;

            if (charLoc.Item2 == MIDDLE_POS)
            {
                if (charLoc.Item1 % 2 == 0)
                    distance += NO_CROSSING_EVEN;
                else
                    distance += NO_CROSSING_ODD;
                cPos = finalPos;
            }

            else if (SINGLE_CROSSING_POS.Contains(charLoc.Item2))
            {
                if (charLoc.Item2 < MIDDLE_POS){
                    int newPos  = (charLoc.Item1 - 1)<0? MIN_GRID: charLoc.Item1 - 1;
                    cPos = mPosToLoc[newPos];
                }
                else if(charLoc.Item2 > MIDDLE_POS){
                    int newPos = (charLoc.Item1 + 1) < MAX_GRID ? MIN_GRID : charLoc.Item1 + 1;
                    cPos = mPosToLoc[newPos];
                }
                distance += SINGLE_CROSSING;
            }
            else if (DOUBLE_CROSSING_POS.Contains(charLoc.Item2))
            {
                if (charLoc.Item2 < MIDDLE_POS)
                {
                    int newPos = (charLoc.Item1 - 1) < 0 ? MIN_GRID : charLoc.Item1 - 1;
                    cPos = mPosToLoc[newPos];
                }
                else if (charLoc.Item2 > MIDDLE_POS)
                {
                    int newPos = (charLoc.Item1 + 1) < MAX_GRID ? MIN_GRID : charLoc.Item1 + 1;
                    cPos = mPosToLoc[newPos];
                }
                distance += DOUBLE_CROSSING;
            }

            return distance;
        }

        private double computeDistance(Tuple<int, int> charLoc1, Tuple<int, int> charLoc2) {
            double distance = 0.0f;

            distance += computeEuclidianCost(cPos, mPosToLoc[charLoc1.Item1]);
            distance += computeCrossingCost(charLoc1, mPosToLoc[charLoc1.Item1]);
      
            distance += computeEuclidianCost(cPos, mPosToLoc[charLoc2.Item1]);
            distance += computeCrossingCost(charLoc2, mPosToLoc[charLoc2.Item1]);

            return distance;
        }

        public double getDistance(String char1, String char2) {
            double distance = 0.0f;

            Tuple<int, int> charLoc1 = getLocation(char1);
            Tuple<int, int> charLoc2 = getLocation(char2);

            distance = computeDistance(charLoc1, charLoc2);

            return distance;
        }

        public void swapElements(String char1, String char2) {
            Tuple<int, int> p1 = getLocation(char1);
            Tuple<int, int> p2 = getLocation(char2);
            mCKeyboard[p1.Item1][p1.Item2] = char2;
            mCKeyboard[p2.Item1][p2.Item2] = char1;
        }

        public void hardReset() {
            mCKeyboard = getDefaultLayout();
            resetCursor();
        }

        public void resetCursor() {
            cPos = new Tuple<float, float>(0, 0);
        }


        private Tuple<int, int> getLocation(String inChar) {
            Tuple<int, int> defaultTuple = new Tuple<int, int>(-1,-1);

            foreach (int key in mCKeyboard.Keys)
            {
                if (mCKeyboard[key].Contains(inChar))
                {
                    return new Tuple<int, int>(key, mCKeyboard[key].IndexOf(inChar));
                }
            }
                    return defaultTuple;
        }

    }
}
