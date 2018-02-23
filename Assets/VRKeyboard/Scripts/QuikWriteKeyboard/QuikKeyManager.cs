using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour {

	public int maxInputLength;

	[Header("UI Elements")]
	public Text inputText;

	[Header("Essentials")]
	public Transform KeySet;
	public Transform TriggerBorders;

	private string Input {
		get { return inputText.text;  }
		set { inputText.text = value;  }
	}

	private Dictionary<GameObject, Text> keysDictionary = new Dictionary<GameObject, Text>();

	private void Awake() {

		for (int i = 0; i < KeySet.childCount; i++) {
			GameObject key = KeySet.GetChild (i).gameObject;
			if (key.tag == "VRGazeInteractable") {
				Text ctext = key.GetComponentInChildren<Text> ();
				keysDictionary.Add(key, ctext);

				key.GetComponent<Button>().onClick.AddListener(() => {
					//GenerateInput(_text.ctext);
				});
			}
		}
			
	}

	public void GenerateInput(string s) {
		if (Input.Length > maxInputLength) { return; }
		Input += s;
	}
}
