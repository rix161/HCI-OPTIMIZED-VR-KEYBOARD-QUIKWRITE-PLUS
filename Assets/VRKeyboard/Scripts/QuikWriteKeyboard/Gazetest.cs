﻿using System.Collections;
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
			m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.blue; 

		}

		private void HandleOut(){
			m_InteractiveItem.GetComponent<Renderer> ().material.color = Color.red;
			//Debug.Log ("Name:" + m_InteractiveItem.name);
			if (m_InteractiveItem.tag.Contains ("Enter")) {
				mVRDevice.onEvent (m_InteractiveItem.name);
			} else if (m_InteractiveItem.tag.Contains ("Inner")) {
				if(m_InteractiveItem.transform.parent.parent.tag.CompareTo("KeyContainer")==0)
					mVRDevice.onEvent ("10"+m_InteractiveItem.name);
			}

			/*else if (m_InteractiveItem.tag.Contains ("Exit")) {
				if(m_InteractiveItem.transform.parent.parent.tag.CompareTo("KeyContainer")==0)
					mVRDevice.onEvent (m_InteractiveItem.transform.parent.parent.name+m_InteractiveItem.name);
			}*/

		}
	}
}
