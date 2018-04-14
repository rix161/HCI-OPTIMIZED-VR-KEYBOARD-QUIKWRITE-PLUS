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
			//m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.blue;
			Debug.Log ("Name:" + m_InteractiveItem.name);

			if(m_InteractiveItem.tag.Contains("Zero") || m_InteractiveItem.tag.Contains ("Enter")){
				if(m_InteractiveItem.tag.Contains("Enter"))
					m_InteractiveItem.GetComponent<Renderer>().material.color = Color.blue;
				mVRDevice.onEvent (m_InteractiveItem.name,false);
			}
			else if (m_InteractiveItem.tag.Contains ("Inner")) {
					m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.blue;
					int.TryParse (m_InteractiveItem.name, out code);
					string temp = (100 + code).ToString ();
				mVRDevice.onEvent (temp,false);
			} else if (m_InteractiveItem.tag.Contains("Button")) {
				mVRDevice.onEvent (m_InteractiveItem.name, true);
			}
		}

		private void HandleOut(){
			if (m_InteractiveItem.GetComponent<Renderer> () != null)
				m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.white;

		}
	}
}
