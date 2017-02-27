using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {
	public int loadStatus; 
	public GameObject message; 
	Text messageTxt;
	bool deleteFile;

	void Awake(){
		messageTxt = message.GetComponentInChildren<Text> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadGame(){
		// Load Status: 0 - no game file found
		//              1 - game file found
		if (loadStatus == 1) {
			message.SetActive (true); 
			messageTxt.text = "Loading Game...";
		} else {
			message.SetActive (true);
			messageTxt.text = "There is no game found. Please select new game."; 
		}
	}

	public void NewGame(){
		if (loadStatus == 1) {
			message.SetActive (true);
			messageTxt.text = "Are you sure you want to delete current game file?";
			if (deleteFile) {
				loadStatus = 0;
			} else {
				loadStatus = 1;
			}
		} else {
			message.SetActive (true);
			messageTxt.text = "Creating New Game..."; 
		}
	}

	public void YesButton(){
		deleteFile = true;
	}

	public void NoButton(){
		deleteFile = false;
	}
}
