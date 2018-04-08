using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace VRStandardAssets.Utils
{
    // This class exists to setup the device on a per platform basis.
    // The class uses the singleton pattern so that only one object exists.
    public class VRDeviceManager : MonoBehaviour
    {
        [SerializeField] private float m_RenderScale = 1.4f;
		[Header("UI Elements")] 
		public Text inputText; 
		public GameObject PredictedWord;
		public GameObject opt1, opt2, opt3;

        private static VRDeviceManager s_Instance;
		private bool zoneEntered = false;
		private int processedCode = 0;
		private int enterCode;
		private int exitCode;
		private int gridCross;
		private int innerGridCross;

		private int state = -1;
		private int lastZone = -1;
		private SymSpell symSpell;

		char [,]QuikWritting = new char[8,5]
							  { {'A','K','S','M','Q'},
							  {'E','H',' ',' ','C'},
							  {'O','V','W','G','Z'},
							  {' ',' ',' ',' ',' '},
							  {'I','B','D','R','J'},
							  {'T','Y',' ',' ','U'},
							  {'N','X','L','F','P'},
							  {' ',' ',' ',' ',' '}};
		List<int> zoneList = new List<int>();
		bool init = false;

		private void codeToChar(int enterCode,int innerGridCross, int exitCode){
			string retString = "";
			enterCode = enterCode - 1;
			exitCode = exitCode - 1;
			int i = enterCode;

			int interCode = enterCode - exitCode;

			if (enterCode == 0 && exitCode == 7) {
				interCode = 1;
			} else if (enterCode == 7 && exitCode == 0) {
				interCode = -1;
			}
			int j = interCode;
			if (innerGridCross != 0) {
				Debug.Log ("GFX:1 innerGridCross: j:" + j);
				if (j < 0)
					j--;
				else
					j++;
				Debug.Log ("GFX:2 innerGridCross: j:" + j);
			}

			//int j = interCode + innerGridCross;
			j = j<0?(j+5):j;
			Debug.Log ("GFX enter:" + enterCode + " exit" + exitCode+" innerGrid:"+innerGridCross);
			Debug.Log ("GFX I:" + i + " J:" + j);
			if(i>=0 && i<QuikWritting.GetLength(0) && j>=0 && j<QuikWritting.GetLength(1))
				retString = ""+QuikWritting [i,j];
		
			//backspace key
			if(enterCode == 7 && exitCode == 7)
				inputText.text = inputText.text.Substring(0,(inputText.text.Length - 1));
			else
				inputText.text += retString;

			if (inputText.text.Length > 15)
				inputText.text = "";
		}

		private void populatePrediction(string text){
			string[] words = text.Split (' ');
			string lastWord = words [words.Length - 1];

			if(lastWord!=null && symSpell!=null){
				var suggestions = symSpell.Lookup (lastWord, SymSpell.Verbosity.All, 3);
				int i = 0;
				if (suggestions.Count > 0) {
					PredictedWord.name = suggestions [0].term;
					Text txtTag = PredictedWord.GetComponentInChildren<Text> ();
					if (txtTag != null)
						txtTag.text = suggestions [0].term;
				}
				HashSet<char> cSet = new HashSet<char>();
				while (i < suggestions.Count && cSet.Count < 3) {
					string sug = suggestions [i++].term;
					if(sug.Length>lastWord.Length && sug [sug.Length - 1]!= '\0')
						cSet.Add (sug [sug.Length - 1]);
				}

				i =0;
				foreach(char c  in cSet){
					GameObject cGameObject = null;

					switch (i) {
					case 1:
						cGameObject = opt1;
						break;
					case 2:
						cGameObject = opt2;
						break;
					case 3:
						cGameObject = opt3;
						break;
					}

					if (cGameObject != null) {
						cGameObject.name = "" + c;
						Text txtText = cGameObject.GetComponentInChildren<Text> ();
						if (txtText != null)
							txtText.text = "" + c; 
						i++;
					}
				}
			}
		}

		public void onEvent(string mString,bool isPrediction){
			int EXZONE = 9;
			int code;


			if (isPrediction) {
				inputText.text += mString;
				populatePrediction (inputText.text);
				return;
			}

			int.TryParse (mString, out code);
			if (code == 0) {
				init = true;
				if (!zoneEntered)
					return;
			
				zoneEntered = !zoneEntered;
				if (zoneList.Count == 1) {
					Debug.Log ("GFX: single Value:" + zoneList [0]);
					codeToChar (zoneList [0], innerGridCross, zoneList [0]);
				} else {
					Debug.Log ("GFX: Two Values:" + zoneList [0]+" :"+zoneList[1]);
					codeToChar (zoneList [0], innerGridCross, zoneList [1]);
				}

				populatePrediction (inputText.text);

				innerGridCross = 0;
				lastZone = 0;
				zoneList.Clear ();
			}
			else if (code >0 && code < EXZONE && init) {
				zoneEntered = true;

				if (code != lastZone) {
					Debug.Log ("GFX: adding Code:" + code+" to: ");
					foreach (int val in zoneList) {
						Debug.Log ("GFX:+" + val);
					}
					zoneList.Add (code);
					lastZone = code;
				}
				/*if (!zoneEntered) {
					exitCode = code;
					codeToChar (enterCode,innerGridCross, exitCode);
					enterCode = 0;
					exitCode = 0;
					gridCross = 0;
					innerGridCross = 0;

				} else {
					enterCode = code;
				}*/
			} else {
				innerGridCross = code - 100;
			}
		}

		public void initDict(){
			int initialCapacity = 20000;
			int maxEditDistanceDictionary = 2; //maximum edit distance per dictionary precalculation
			symSpell = new SymSpell(initialCapacity, maxEditDistanceDictionary);
			TextAsset dictionaryPath = Resources.Load<TextAsset>("dataset");
			int termIndex = 0; //column of the term in the dictionary text file
			int countIndex = 1; //column of the term frequency in the dictionary text file
			if (!symSpell.LoadDictionary(dictionaryPath, termIndex, countIndex)){
				Debug.Log ("Unable to load dictionary");    
			}
		}

        public static VRDeviceManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                  	DontDestroyOnLoad (s_Instance.gameObject);
                }

                return s_Instance;
            }
        }


        private void Awake ()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
				initDict ();
                DontDestroyOnLoad (this);
            }
            else if (this != s_Instance)
            {
                Destroy (gameObject);
            }
        }
    }
}