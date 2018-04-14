using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VRStandardAssets.Utils
{
    // In order to interact with objects in the scene
    // this class casts a ray into the scene and if it finds
    // a VRInteractiveItem it exposes it for other classes to use.
    // This script should be generally be placed on the camera.
    public class VREyeRaycaster : MonoBehaviour
    {
        public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.


        [SerializeField] private Transform m_Camera;
        [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
        [SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
        [SerializeField] private VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
        [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
        [SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
        [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
        [SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

        
        private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
        private VRInteractiveItem m_LastInteractible;                   //The last interactive item

		#region Public Variables
		public float loadingTime;
		public Image circle;
		public GameObject myObj;
		#endregion



        // Utility for other classes to get the current interactive item
        public VRInteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        
        private void OnEnable()
        {
			if (m_VrInput != null) {
				m_VrInput.OnClick += HandleClick;
				m_VrInput.OnDoubleClick += HandleDoubleClick;
				m_VrInput.OnUp += HandleUp;
				m_VrInput.OnDown += HandleDown;
			}
        }


        private void OnDisable ()
        {
			if (m_VrInput != null) {
				m_VrInput.OnClick -= HandleClick;
				m_VrInput.OnDoubleClick -= HandleDoubleClick;
				m_VrInput.OnUp -= HandleUp;
				m_VrInput.OnDown -= HandleDown;
			}
        }


        private void Update()
        {
            EyeRaycast();
        }



		#region Private Methods
		private IEnumerator FillCircle(VRInteractiveItem target,float loadingTime) {
			// When the circle starts to fill, reset the timer.
			float timer = 0f;
			circle.fillAmount = 0f;

			while (timer < loadingTime) {
				if (m_LastInteractible!=null && target.name != m_LastInteractible.name) {
					yield break;
				}

				timer += Time.deltaTime;
				circle.fillAmount = timer / loadingTime;
				yield return null;
			}

			circle.fillAmount = 1f;

			target.Over ();
			ResetGazer();
		}

		// Reset the loading circle to initial, and clear last detected target.
		private void ResetGazer() {
			if (circle == null) {
				Debug.LogError("Please assign target loading image, (ie. circle image)");
				return;
			}

			circle.fillAmount = 0f;
		}
		#endregion



      
        private void EyeRaycast()
        {
            // Show the debug ray if required
            if (m_ShowDebugRay)
            {
                Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
            }

            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;
			RaycastHit []hits;
			VRInteractiveItem cInteractable = null;
			int hitIndex = -1;

			hits = Physics.RaycastAll (ray);
			foreach (RaycastHit hitDash in hits) {
				hitIndex++;
				VRInteractiveItem interactible = hitDash.collider.GetComponent<VRInteractiveItem>();
				if (interactible != null) {
					cInteractable = interactible;

					if (cInteractable.tag.Contains ("Button") || cInteractable.tag.Contains("Inner"))
						break;
				}
			} 
            // Do the raycast forweards to see if we hit an interactive item
			if (cInteractable!=null)
            {
				VRInteractiveItem interactible = cInteractable;
				m_CurrentInteractible = interactible;
				//Debug.Log ("GFX: VREyeRaycaster:" + interactible.transform.name);
				float timerDuation = 4.0f;
				float peakFactor = 0.25f;
                // If we hit an interactive item and it's not the same as the last interactive item, then call Over
				if (interactible && interactible != m_LastInteractible) {
					//Debug.Log ("VREyeRaycaster:" + interactible.transform.name);
					if (interactible != null && interactible.transform.tag.Contains ("Button")) { 
						m_LastInteractible = interactible;
						if(interactible.tag.Contains("ButtonWord"))
							StartCoroutine (FillCircle (interactible,timerDuation));
						else
							StartCoroutine (FillCircle (interactible,timerDuation*peakFactor));
					} else {
						ResetGazer ();
						interactible.Over ();
					}

				}

                // Deactive the last interactive item 
                if (interactible != m_LastInteractible)
                    DeactiveLastInteractible();


                m_LastInteractible = interactible;

                // Something was hit, set at the hit position.
                if (m_Reticle)
					m_Reticle.SetPosition(hits[hitIndex]);

                if (OnRaycasthit != null)
					OnRaycasthit(hits[hitIndex]);
            }
            else
            {
                // Nothing was hit, deactive the last interactive item.
                DeactiveLastInteractible();
                m_CurrentInteractible = null;
				ResetGazer();

                // Position the reticle at default distance.
                if (m_Reticle)
                    m_Reticle.SetPosition();
            }
        }


        private void DeactiveLastInteractible()
        {
            if (m_LastInteractible == null)
                return;

            m_LastInteractible.Out();
            m_LastInteractible = null;
        }


        private void HandleUp()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Up();
        }


        private void HandleDown()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Down();
        }


        private void HandleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Click();
        }


        private void HandleDoubleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.DoubleClick();

        }
    }
}