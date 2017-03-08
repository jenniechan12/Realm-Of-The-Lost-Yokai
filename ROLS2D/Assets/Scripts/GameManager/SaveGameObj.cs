using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameObj : MonoBehaviour {
	void Awake(){
		DontDestroyOnLoad (gameObject);
	}
}
