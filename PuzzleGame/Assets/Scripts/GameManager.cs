using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	int currentPuzzle;

	void Awake()
	{
		currentPuzzle = 13 ;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public int CurrentPuzzle
	{
		get{return currentPuzzle;}
		set{currentPuzzle = value;}
	}
}