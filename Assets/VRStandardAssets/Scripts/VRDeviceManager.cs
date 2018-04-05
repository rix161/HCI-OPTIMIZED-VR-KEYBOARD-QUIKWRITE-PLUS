using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using UnityEngine.UI;

namespace VRStandardAssets.Utils
{
    // This class exists to setup the device on a per platform basis.
    // The class uses the singleton pattern so that only one object exists.
    public class VRDeviceManager : MonoBehaviour
    {
        [SerializeField] private float m_RenderScale = 1.4f;
		[Header("UI Elements")] public Text inputText; 

        private static VRDeviceManager s_Instance;
		private bool zoneEntered = false;
		private int processedCode = 0;
		private int enterCode;
		private int exitCode;
		private int gridCross;
		private int innerGridCross;

		private int state = -1;

		char [,]QuikWritting = new char[8,5]
							  { {'A','K','S','Q','M'},
							  {'E','H',' ',' ','C'},
							  {'O','V','W','G','Z'},
							  {' ',' ',' ',' ',' '},
							  {'I','B','D','R','J'},
							  {'T','Y',' ',' ','U'},
							  {'N','X','L','F','P'},
							  {' ',' ',' ',' ',' '}};
							

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

			int j = interCode + innerGridCross;
			j = j<0?(j+5):j;

			if(i>=0 && i<QuikWritting.GetLength(0) && j>=0 && j<QuikWritting.GetLength(1))
				retString = ""+QuikWritting [i,j];

			//backspace key
			if(enterCode == 7 && exitCode == 7)
				inputText.text = inputText.text.Substring(0,(inputText.text.Length - 1));
			else
				inputText.text += retString;

		}

		public void onEvent(string mString){
			int EXZONE = 9;
			int code;
			int.TryParse (mString, out code);
			if (code <= 0) {
				
			}

			if (code < EXZONE) {
				zoneEntered = !zoneEntered;
				if (!zoneEntered) {
					exitCode = code;
					codeToChar (enterCode,innerGridCross, exitCode);
					enterCode = 0;
					exitCode = 0;
					gridCross = 0;
					innerGridCross = 0;

				} else {
					enterCode = code;
				}
			} else {
				innerGridCross = code - 100;
			}
		


		}

        public static VRDeviceManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<VRDeviceManager> ();
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
                DontDestroyOnLoad (this);
            }
            else if (this != s_Instance)
            {
                Destroy (gameObject);
            }
        }
    }
}