using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelect : MonoBehaviour {

	PlayerInventory playerInventory;

	// Use this for initialization
	void Start () {
		playerInventory = GameObject.Find("PlayerInventory").GetComponent<PlayerInventory>();
	}
	
	public void OnMouseDown()
	{
		if(playerInventory)
			playerInventory.SelectItem(gameObject);
	}
}
