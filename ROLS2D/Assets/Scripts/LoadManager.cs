using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour {
	public int loadStatus; 
	public GameObject message, yesButton, noButton; 
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
			yesButton.SetActive (true);
			noButton.SetActive (true);
			if (deleteFile) {
				loadStatus = 0;
				SceneManager.LoadScene (2);
			} else {
				loadStatus = 1;
			}
		} else {
			message.SetActive (true);
			messageTxt.text = "Creating New Game..."; 
			SceneManager.LoadScene (2);
		}
	}

	public void YesButton(){
		deleteFile = true;
		yesButton.SetActive (false);
		noButton.SetActive (false);
	}

	public void NoButton(){
		deleteFile = false;
		yesButton.SetActive (false);
		noButton.SetActive (false);
	}
}
