using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectItem : MonoBehaviour {

	PlayerInventory playerInventory;

	// Use this for initialization
	void Start () {
		playerInventory = GameObject.Find("PlayerInventory").GetComponent<PlayerInventory>();
	}
	
	void OnMouseDown()
	{
		playerInventory.SelectItemClick(gameObject);
	}
}
