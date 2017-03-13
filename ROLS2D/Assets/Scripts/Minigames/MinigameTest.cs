using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTest : MonoBehaviour {

	GameObject battleManagerObj;
	BattleManager battleManager;

	private float timer;

	// Use this for initialization
	void Start () {
		Debug.Log("minigame test started.");

		battleManagerObj = GameObject.Find("BattleManager");
		if (battleManagerObj)
			battleManager = battleManagerObj.GetComponent<BattleManager>();
		if (battleManagerObj == null)
			Debug.Log("Error finding battle manager obj in MinigameTest");
		if (battleManagerObj == null)
			Debug.Log("Error finding battle manager component in MinigameTest");

		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime; 
		if (timer > 2)
		{
			if (battleManager)
				battleManager.MinigameEnd(1, 1);
			Destroy(gameObject);
			Debug.Log("Minigame test ending.");
		}
	}
}
