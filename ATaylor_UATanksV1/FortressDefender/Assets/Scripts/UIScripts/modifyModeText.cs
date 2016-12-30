using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class modifyModeText : MonoBehaviour {

	Text modeText;
	// Use this for initialization
	void Start () {
		modeText = gameObject.GetComponent<Text> ();
		modeText.text = "Current Selection is: " + GameManager.instance.selectMode;
		
	}
	
	// Update is called once per frame
	void Update () {//used to update and display to the user their currently selected
		modeText.text = "Current Selection is: " + GameManager.instance.selectMode;
	}
}
