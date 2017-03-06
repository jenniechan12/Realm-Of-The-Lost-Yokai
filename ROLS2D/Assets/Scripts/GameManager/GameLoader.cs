using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

	public GameObject playerManager; 

	void Awake(){

		// If player manager not found, instantiate it
		if (PlayerManager.instance == null)
			Instantiate (playerManager);
	}
}
