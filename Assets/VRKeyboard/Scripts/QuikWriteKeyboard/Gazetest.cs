using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRStandardAssets.Utils{
	
public class Gazetest: MonoBehaviour {
	[SerializeField] private VRInteractiveItem m_InteractiveItem;
	[SerializeField] private VRDeviceManager mVRDevice;


		private void OnEnable (){
			m_InteractiveItem.OnOver += HandleOver;
			m_InteractiveItem.OnOut += HandleOut;
		}

		private void OnDisable (){
			m_InteractiveItem.OnOver -= HandleOver;
			m_InteractiveItem.OnOut -= HandleOut;

		}
		private void HandleOver (){
			int code;
			m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.red;
			//Debug.Log ("Name:" + m_InteractiveItem.name);
			if (m_InteractiveItem.tag.Contains ("Enter")) {
				mVRDevice.onEvent (m_InteractiveItem.name);
			} else if (m_InteractiveItem.tag.Contains ("Inner")) {
				if (m_InteractiveItem.transform.parent.parent.tag.CompareTo ("KeyContainer") == 0) {

					int.TryParse (m_InteractiveItem.name, out code);
					string temp = (100 + code).ToString ();
					mVRDevice.onEvent (temp);
				}
			
			} else if (m_InteractiveItem.transform.name.Contains ("Start")) {
				
			}


		}

		private void HandleOut(){
			m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.blue; 
		}
	}
}
