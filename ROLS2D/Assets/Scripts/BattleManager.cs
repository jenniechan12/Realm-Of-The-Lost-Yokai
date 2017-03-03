﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

	// Minigame Resources
	GameObject MINIGAMETESTRESOURCE;

	// Main Option Buttons
	GameObject fightButton;
	GameObject fleeButton;
	GameObject talkButton;
	GameObject itemButton;

	private PlayerInventory inventory;
	
	// Use this for initialization
	void Start () 
	{
		MINIGAMETESTRESOURCE = Resources.Load("Prefab/MinigameTest") as GameObject;
		if (MINIGAMETESTRESOURCE == null)
			Debug.Log("Error loading minigame test resource in battlemanager");

		// Find buttons
		fightButton = GameObject.Find("FightButton");
		fleeButton = GameObject.Find("FleeButton");
		talkButton = GameObject.Find("ItemButton");
		itemButton = GameObject.Find("TalkButton");

		if (fightButton == null)
			Debug.Log("Cannot find fight button object in BattleManager.");
		if (talkButton == null)
			Debug.Log("Cannot find talk button object in BattleManager.");
		if (fleeButton == null)
			Debug.Log("Cannot find flee button object in BattleManager.");
		if (itemButton == null)
			Debug.Log("Cannot find item button object in BattleManager.");

		inventory = GameObject.Find("PlayerInventory").GetComponent<PlayerInventory>();
		if (inventory == null)
			Debug.Log("Cannot find player inventory in BattleManager start.");

		// Tell the inventory that we are in battle
		if (inventory)
			inventory.Battling = true;
	}

	void OnDestroy()
	{	
		// Tell the inventory that we are no longer in battle
		if (inventory)
			inventory.Battling = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void FightClick()
	{
		Debug.Log("Fight selected.");
		HideButtons();
		GameObject minigame = Instantiate(MINIGAMETESTRESOURCE, Vector3.zero, Quaternion.identity);
	}

	public void FleeClick()
	{
		Debug.Log("Flee selected.");

	}

	public void ItemClick()
	{
		// DEBUGGING print inventory
		if (inventory)
		{
			HideButtons();
			inventory.Display();
		}
	}

	public void TalkClick()
	{
		Debug.Log("Talk selected.");
		inventory.AddItem("TestPotion4", 1);
	}

	public void HideButtons()
	{
		Debug.Log("Hiding buttons");
		fightButton.SetActive(false);
		talkButton.SetActive(false);
		fleeButton.SetActive(false);
		itemButton.SetActive(false);
	}

	public void UnhideButtons()
	{
		Debug.Log("Unhiding buttons");
		fightButton.SetActive(true);
		talkButton.SetActive(true);
		fleeButton.SetActive(true);
		itemButton.SetActive(true);
	}

	public void MinigameEnd()
	{
		// Change state
		UnhideButtons();
	}


}
