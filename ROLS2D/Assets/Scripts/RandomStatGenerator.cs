using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStatGenerator : MonoBehaviour {
	public Transform[] attributes;
	int randNum; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void RandomNumberGenerator(){
		randNum = Random.Range (1, 6);
	}

	public void RandomStat(){
		for (int i = 0; i < attributes.Length; i++) {
			RandomNumberGenerator ();
			attributes [i].localScale = new Vector3 (randNum, 1, 1);
		}
	}
}
