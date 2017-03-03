// NOTE: 3/1/17 Individual list types need to be created 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class PlayerInventory : MonoBehaviour {

	private GameObject itemDatabaseObj;
	private ItemDatabase itemDatabase;

	private List<BaseItem> playerInventory = new List<BaseItem>();
	public TextAsset inventoryText;

	private int money;
	private const int MAX_MONEY = 100000;


	void Start()
	{
		// References to objects and components
		itemDatabaseObj = GameObject.Find("ItemDatabase");
		if (itemDatabaseObj)
			itemDatabase = itemDatabaseObj.GetComponent<ItemDatabase>();
		if (itemDatabaseObj == null)
			Debug.Log("Cannot find item database object in player inventory start.");
		if (itemDatabase == null)
			Debug.Log("Cannot find item database component in player inventory start.");

		// Load items from xml into inventory
		LoadInvetory();
	}


	// ITEM FUNCTIONS

	// Add to or increase inventory count
	public void AddItem(string name, int count)
	{
		// Check to see if the player already has the item
		foreach (BaseItem item in playerInventory)
		{
			if (name == item.Name)
			{
				item.Count += count;
				return;
			}
		}

		// Copy item from database and set count
		BaseItem tempItem = new BaseItem();

		if (itemDatabase != null)
		{
			tempItem = itemDatabase.GetItem(name);
		}

		tempItem.Count = count;
		playerInventory.Add(tempItem);
	}

	// Decrease item in invenotry by count
	public void SubtractItem(string name, int count)
	{
		int i = 0;
		for (i = 0; i<playerInventory.Count; i++)
		{
			if (name == playerInventory[i].Name)
			{
				playerInventory[i].Count -= count;
				break;
			}
		}
		if (i<playerInventory.Count)	// Make sure item exists
		{
			if (playerInventory[i].Count <= 0)
			{
				playerInventory.Remove(playerInventory[i]);
			}
		}
	}


	// MONEY FUNCTIONS

	// Increase money by amount
	public void AddMoney(int amount)
	{
		// Add a check for max amount
		money += amount;
		if (money > MAX_MONEY)
			money = MAX_MONEY;
	}

	// Decrease money by amount
	public void SubtractMoney(int amount)
	{
		money -= amount;
		if (money < 0)
			money = 0;
	}


	// Loads inventory from xml file
	void LoadInvetory()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(inventoryText.text);
		XmlNodeList itemNodeList = xmlDocument.GetElementsByTagName("Item");

		// Read items from xml, copy from database, and add to inventory
		foreach (XmlNode itemInfo in itemNodeList)
		{
			XmlNodeList itemContent = itemInfo.ChildNodes;
			string tempName = "";	
			string tempCount = "";

			foreach(XmlNode content in itemContent)
			{
				switch(content.Name)
				{
					case "ItemName":
						tempName = content.InnerText;
						break;
					case "ItemCount":
						tempCount = content.InnerText;
						break;
				}
			}
			BaseItem tempItem = new BaseItem();
			// Copy item from database and set count
			if (itemDatabase != null)
			{
				tempItem = itemDatabase.GetItem(tempName);
			}

			tempItem.Count = int.Parse(tempCount);
			playerInventory.Add(tempItem);
			
		}
	}


	// For debugging
	public void PrintInventory()
	{	
		Debug.Log("Printing player inventory:");

		foreach (BaseItem item in playerInventory)
		{
			Debug.Log(item.Name);
			Debug.Log(item.Count);
		}
	}
}
