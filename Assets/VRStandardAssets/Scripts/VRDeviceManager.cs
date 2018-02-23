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

		char [,]QuikWritting = new char[8,5]
							  { {'A','K','S','M','Q'},
							  {'E','H',' ',' ','C'},
							  {'O','V','W','G','Z'},
							  {' ',' ',' ',' ',' '},
							  {'I','B','D','R','J'},
							  {'T',' ','Y',' ','T'},
							  {'N','X','L','F','P'},
							  {' ',' ',' ',' ',' '}};
							

		private string codeToChar(int enterCode,int innerGridCross, int exitCode){
			string retString = "";
			enterCode = enterCode - 1;
			exitCode = exitCode - 1;
			int i = enterCode;
			if (enterCode == 1 && exitCode == 7) {
				exitCode = 0;
			} else if (enterCode == 7 && exitCode == 1) {
				exitCode = 9;
			}

			int j = enterCode - exitCode + innerGridCross;
			j = j<0?(j+5):j;

			if(i>=0 && i<QuikWritting.GetLength(0) && j>=0 && j<QuikWritting.GetLength(1))
				retString = ""+QuikWritting [i,j];


			return retString;

		}

		public void onEvent(string mString){
			int EXZONE = 9;
			int code;
			int.TryParse (mString, out code);
			if (code <= 0)
				return;

			if (code < EXZONE) {
				zoneEntered = !zoneEntered;
				if (!zoneEntered) {
					exitCode = code;
					string strCode = codeToChar (enterCode,innerGridCross, exitCode);
					inputText.text += strCode;
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