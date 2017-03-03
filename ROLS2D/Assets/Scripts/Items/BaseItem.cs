﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem {

	public enum ItemType {WEAPON, ARMOR, POTION, STORY, DEFAULT};
	protected string itemName;
	protected string itemDescription;
	protected int itemID;
	protected int itemCount;
	protected ItemType itemType;
	protected string itemPrefab;	// Name of item prefab

	// Constructor for Item Database
	public BaseItem(Dictionary<string, string> dictionary)
	{
		// Set values from dictionary
		itemName = dictionary["ItemName"];
		itemID = int.Parse(dictionary["ItemID"]);
		itemCount = int.Parse(dictionary["ItemCount"]);
		itemDescription = dictionary["ItemDescription"];
		itemType = (ItemType)System.Enum.Parse(typeof(BaseItem.ItemType), dictionary["ItemType"].ToString());
		itemPrefab = dictionary["ItemPrefab"];
	}

	// Default constructor
	public BaseItem()
	{
		// Set default values
		itemName = "";
		itemID = 0;
		itemCount = 1;
		itemDescription = "";
		itemType = ItemType.DEFAULT;
	}

	// Copy constructor
	public BaseItem(BaseItem item)
	{
		// Set default values
		itemName = item.Name;
		itemID = item.ID;
		itemCount = item.Count;
		itemDescription = item.Description;
		itemType = item.Type;
	}

	// Get and Set Functions
	public string Name
	{
		get {return itemName;}
		set {itemName = value;}
	}
	
	public string Description
	{
		get {return itemDescription;}
		set {itemDescription = value;}
	}

	public int ID
	{
		get {return itemID;}
		set {itemID = value;}
	}

	public int Count
	{
		get {return itemCount;}
		set {itemCount = value;}
	}

	public ItemType Type
	{
		get {return itemType;}
		set {itemType = value;}
	}

	public string Prefab
	{
		get {return itemPrefab;}
		set {itemPrefab = value;}
	}
}
