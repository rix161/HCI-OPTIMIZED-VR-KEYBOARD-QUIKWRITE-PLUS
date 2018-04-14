using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
		private static SymSpell symSpell;

		char [,]QuikWritting = new char[8,5]
							  { {'A','K','S','M','Q'},
							  {'E','H',' ',' ','C'},
							  {'O','V','W','G','Z'},
							  {' ',' ',' ',' ',' '},
							  {'I','B','D','R','J'},
							  {'T','Y',' ',' ','U'},
							  {'N','X','L','F','P'},
							  {' ',' ',' ',' ',' '}};
		char [,]QuikWrittingplus = new char[8,5]
			{ {'E','O','J','W','M'},
			{'A','Y',' ',' ','S'},
			{'R','K','P','D','Z'},
			{' ',' ',' ',' ',' '},
			{'N','C','B','V','G'},
			{'L','U',' ',' ','H'},
			{'I','T','Q','X','F'},
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
				//Debug.Log ("GFX:1 innerGridCross: j:" + j);
				if (j < 0)
					j--;
				else
					j++;
				//Debug.Log ("GFX:2 innerGridCross: j:" + j);
			}

			//int j = interCode + innerGridCross;
			j = j<0?(j+5):j;
			//Debug.Log ("GFX enter:" + enterCode + " exit" + exitCode+" innerGrid:"+innerGridCross);
			//Debug.Log ("GFX I:" + i + " J:" + j);
			if(i>=0 && i<QuikWrittingplus.GetLength(0) && j>=0 && j<QuikWritting.GetLength(1))
				retString = ""+QuikWrittingplus [i,j];
		
			//backspace key
			if(enterCode == 7 && exitCode == 7)
				inputText.text = inputText.text.Substring(0,(inputText.text.Length - 1));
			else
				inputText.text += retString;

			if (inputText.text.Length > 50)
				inputText.text = "";
		}

		private IEnumerator populatePrediction(string text){
			string[] words = text.Split (' ');
			string lastWord = words [words.Length - 1];
			lastWord = lastWord.ToLower ();
			if (symSpell == null)
				initDict ();
			if(lastWord!=null && symSpell!=null){
				Debug.Log ("GFX: LastWord:" + lastWord);
				List<SymSpell.SuggestItem> suggestions = symSpell.Lookup (lastWord, SymSpell.Verbosity.All, 3);
				suggestions.Sort ();
				Text txtTag = PredictedWord.GetComponentInChildren<Text> ();
				if (suggestions != null) {
					txtTag.text = suggestions [0].term.ToUpper();
					PredictedWord.name = suggestions [0].term.ToUpper ();
				}

				List<SymSpell.SuggestItem> fitered1 = suggestions;
				/*List<SymSpell.SuggestItem> filtered = suggestions.Where(s => (s.term.Length>text.Length)).ToList();
				filtered = filtered.Where(s => ((s.term.IndexOf(text)==0))).ToList();
				filtered.Sort ();
				List<SymSpell.SuggestItem> fitered1, fitered2;
				if (filtered.Count<= 0) {
					Debug.Log ("GFX: using Default List");
					fitered1 = suggestions.Where (s => (s.distance == 1)).ToList ();
					fitered2 = suggestions.Where (s => (s.distance == 2)).ToList ();
				}
				else{
					Debug.Log ("GFX: using Filtered List");
					fitered1 = filtered.Where (s => (s.distance == 1)).ToList ();
					fitered2 = filtered.Where (s => (s.distance == 2)).ToList ();
				}
				int i = 0;
				if (fitered2.Count > 0) {
					Debug.Log ("GFX: LastWord:"+lastWord+" predicted word:"+fitered2 [0].term);
					PredictedWord.name = fitered2 [0].term;
					Text txtTag = PredictedWord.GetComponentInChildren<Text> ();
					if (txtTag != null)
						txtTag.text = fitered2 [0].term.ToUpper();
				}*/


				int i = 0;
				HashSet<char> cSet = new HashSet<char>();
				while (i < fitered1.Count && cSet.Count < 3) {
					string sug = fitered1 [i++].term;
					Debug.Log ("GFX: Filter1: Sug" + sug + " last:" + sug [sug.Length - 1]);
					if(sug.Length>lastWord.Length && sug [sug.Length - 1]!= '\0')
						cSet.Add (sug [sug.Length - 1]);
				}

				i =1;
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
						if (txtText != null) {
							txtText.text = "" + c.ToString ().ToUpper (); 
							cGameObject.name = "" + c.ToString ().ToUpper ();
						}
						i++;
					}
				}
			}
			yield return null;
		}

		private string getLastWord(string text){
			string[] words = text.Split (' ');
			string lastWord = words [words.Length - 1];
			lastWord = lastWord.ToLower ();
			return lastWord;
		}

		public void onEvent(string mString,bool isPrediction){
			int EXZONE = 9;
			int code;


			if (isPrediction) {
				if (mString.Length > 1) {
					string lastWord = getLastWord (inputText.text);
					Debug.Log ("GFX VRDeviceManager: Before" + inputText.text);
					inputText.text = inputText.text.Substring (0,inputText.text.Length - lastWord.Length);
					Debug.Log ("GFX VRDeviceManager:After: LastWord:" + lastWord + " " + inputText.text);
				}
				inputText.text += mString.ToUpper();
				StartCoroutine(populatePrediction (inputText.text));
				return;
			}

			int.TryParse (mString, out code);
			if (code == 0) {
				init = true;
				if (!zoneEntered)
					return;
			
				zoneEntered = !zoneEntered;
				if (zoneList.Count == 1) {
					//Debug.Log ("GFX: single Value:" + zoneList [0]);
					codeToChar (zoneList [0], innerGridCross, zoneList [0]);
				} else {
					//Debug.Log ("GFX: Two Values:" + zoneList [0]+" :"+zoneList[1]);
					codeToChar (zoneList [0], innerGridCross, zoneList [1]);
				}

				StartCoroutine(populatePrediction (inputText.text));

				innerGridCross = 0;
				lastZone = 0;
				zoneList.Clear ();
			}
			else if (code >0 && code < EXZONE && init) {
				zoneEntered = true;

				if (code != lastZone) {
					//Debug.Log ("GFX: adding Code:" + code+" to: ");
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
			int maxEditDistanceDictionary = 3; //maximum edit distance per dictionary precalculation
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