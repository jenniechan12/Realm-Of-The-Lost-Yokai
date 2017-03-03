using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class PlayerInventory : MonoBehaviour
{

	private GameObject itemDatabaseObj;
	private ItemDatabase itemDatabase;

	private List<GameObject> activeObjects = new List<GameObject>();
	private List<Vector3> menuLocations = new List<Vector3>();

	private List<WeaponItem> playerWeaponItems = new List<WeaponItem> ();
	private List<ArmorItem> playerArmorItems = new List<ArmorItem> ();
	private List<PotionItem> playerPotionItems = new List<PotionItem> ();
	private List<StoryItem> playerStoryItems = new List<StoryItem> ();

	public TextAsset inventoryText;

	private int money;
	private const int MAX_MONEY = 100000;

	private const int MAX_ITEMS = 20;

	bool battling;	// Are we displaying in a battle or from world?
	bool displaying;

	void Awake()
	{
		battling = false;
		displaying = false;
	}

	void Start ()
	{
		// References to objects and components
		itemDatabaseObj = GameObject.Find ("ItemDatabase");
		if (itemDatabaseObj)
			itemDatabase = itemDatabaseObj.GetComponent<ItemDatabase> ();
		if (itemDatabaseObj == null)
			Debug.Log ("Cannot find item database object in player inventory start.");
		if (itemDatabase == null)
			Debug.Log ("Cannot find item database component in player inventory start.");

		// Load items from xml into inventory
		LoadInvetory();
		// Instantiate and set item prefab locations
		LoadPrefabs();
		SetMenuLocations();
		UpdateDisplay();
	}


	//************************************************************
	// ITEM FUNCTIONS
	//************************************************************
	// Add to or increase inventory count
	public void AddItem (string name, int count)
	{
		// Check to see if the player already has the item
		foreach (WeaponItem item in playerWeaponItems) {
			if (name == item.Name) {
				item.Count += count;
				return;
			}
		}
		foreach (ArmorItem item in playerArmorItems) {
			if (name == item.Name) {
				item.Count += count;
				return;
			}
		}
		foreach (PotionItem item in playerPotionItems) {
			if (name == item.Name) {
				item.Count += count;
				return;
			}
		}
		foreach (StoryItem item in playerStoryItems) {
			if (name == item.Name) {
				item.Count += count;
				return;
			}
		}

		Dictionary<string, string> tempDict;
		tempDict = itemDatabase.GetItem (name);
		if (tempDict != null) 
		{
			Object ITEM;
			GameObject tempItem;
			switch (tempDict ["ItemType"]) 
			{
				case "WEAPON":
					WeaponItem tempWeapon = new WeaponItem (tempDict);
					tempWeapon.Count = count;
					playerWeaponItems.Add (tempWeapon);
					ITEM = Resources.Load("Prefab/" + tempWeapon.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					break;
				case "ARMOR":
					ArmorItem tempArmor = new ArmorItem (tempDict);
					tempArmor.Count = count;
					playerArmorItems.Add (tempArmor);
					ITEM = Resources.Load("Prefab/" + tempArmor.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					break;
				case "POTION":
					PotionItem tempPotion = new PotionItem (tempDict);
					tempPotion.Count = count;
					playerPotionItems.Add (tempPotion);
					ITEM = Resources.Load("Prefab/" + tempPotion.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					break;
				case "STORY":
					StoryItem tempStory = new StoryItem (tempDict);
					tempStory.Count = count;
					playerStoryItems.Add (tempStory);
					ITEM = Resources.Load("Prefab/" + tempStory.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					break;
			}
			UpdateDisplay();
		}
	}

	// Decrease item in invenotry by count
	public void SubtractItem (string name, int count)
	{
		int i = 0;
		for (i = 0; i < playerWeaponItems.Count; i++) 
		{
			if (playerWeaponItems[i].Name == name)
			{
				playerWeaponItems[i].Count -= count;
			}
		}
		for (i = 0; i < playerArmorItems.Count; i++) 
		{
			if (playerArmorItems[i].Name == name)
			{
				playerArmorItems[i].Count -= count;
			}
		}
		for (i = 0; i < playerPotionItems.Count; i++) 
		{
			if (playerPotionItems[i].Name == name)
			{
				playerPotionItems[i].Count -= count;
			}
		}
		for (i = 0; i < playerStoryItems.Count; i++) 
		{
			if (playerStoryItems[i].Name == name)
			{
				playerStoryItems[i].Count -= count;
			}
		}

		// Remove from list is 0 or less
		if (i < playerWeaponItems.Count) 
		{	// Make sure item exists
			if (playerWeaponItems [i].Count <= 0) 
			{
				playerWeaponItems.Remove (playerWeaponItems [i]);
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerWeaponItems[i].Prefab == activeObjects[k].name)
					{
						activeObjects.Remove(activeObjects[k]);
					}
				}
			}
			return;
		}
		if (i < playerArmorItems.Count) 
		{	// Make sure item exists
			if (playerArmorItems [i].Count <= 0) 
			{
				playerArmorItems.Remove (playerArmorItems [i]);
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerWeaponItems[i].Prefab == activeObjects[k].name)
					{
						activeObjects.Remove(activeObjects[k]);
					}
				}
				return;
			}
		}
		if (i < playerPotionItems.Count) 
		{	// Make sure item exists
			if (playerPotionItems [i].Count <= 0) 
			{
				playerPotionItems.Remove (playerPotionItems [i]);
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerWeaponItems[i].Prefab == activeObjects[k].name)
					{
						activeObjects.Remove(activeObjects[k]);
					}
				}
				return;
			}
		}
		if (i < playerStoryItems.Count) 
		{	// Make sure item exists
			if (playerStoryItems [i].Count <= 0) 
			{
				playerStoryItems.Remove (playerStoryItems [i]);
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerWeaponItems[i].Prefab == activeObjects[k].name)
					{
						activeObjects.Remove(activeObjects[k]);
					}
				}
				return;
			}
		}
	}

	//************************************************************
	// MONEY FUNCTIONS
	//************************************************************

	// Increase money by amount
	public void AddMoney (int amount)
	{
		// Add a check for max amount
		money += amount;
		if (money > MAX_MONEY)
			money = MAX_MONEY;
	}

	// Decrease money by amount
	public void SubtractMoney (int amount)
	{
		money -= amount;
		if (money < 0)
			money = 0;
	}


	// Loads inventory from xml file
	void LoadInvetory ()
	{
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (inventoryText.text);
		XmlNodeList itemNodeList = xmlDocument.GetElementsByTagName ("Item");

		// Read items from xml, copy from database, and add to inventory
		foreach (XmlNode itemInfo in itemNodeList) {
			XmlNodeList itemContent = itemInfo.ChildNodes;
			string tempName = "";	
			string tempCount = "";

			foreach (XmlNode content in itemContent) {
				switch (content.Name) {
					case "ItemName":
						tempName = content.InnerText;
						break;
					case "ItemCount":
						tempCount = content.InnerText;
						break;
				}
			}
			Dictionary<string, string> tempDict;
			// Copy item from database and set count
			if (itemDatabase != null) 
			{
				tempDict = itemDatabase.GetItem (tempName);
				if (tempDict != null) 
				{
					switch (tempDict ["ItemType"]) 
					{
						case "WEAPON":
							WeaponItem tempWeapon = new WeaponItem (tempDict);
							tempWeapon.Count = int.Parse(tempCount);
							playerWeaponItems.Add (tempWeapon);
							break;
						case "ARMOR":
							ArmorItem tempArmor = new ArmorItem (tempDict);
							tempArmor.Count = int.Parse(tempCount);
							playerArmorItems.Add (tempArmor);
							break;
						case "POTION":
							PotionItem tempPotion = new PotionItem (tempDict);
							tempPotion.Count = int.Parse(tempCount);
							playerPotionItems.Add (tempPotion);
							break;
						case "STORY":
							StoryItem tempStory = new StoryItem (tempDict);
							tempStory.Count = int.Parse(tempCount);
							playerStoryItems.Add (tempStory);
							break;
						}
				}
			}
		}
	}


	// For debugging
	public void PrintInventory ()
	{	
		Debug.Log ("Printing player inventory:");

		Debug.Log ("Printing player weapons:");
		foreach (WeaponItem item in playerWeaponItems) {
			Debug.Log (item.Name);
			Debug.Log((item.Type).ToString());
			Debug.Log (item.Count);
		}
		Debug.Log ("Printing player armor:");
		foreach (ArmorItem item in playerArmorItems) {
			Debug.Log (item.Name);
			Debug.Log((item.Type).ToString());
			Debug.Log (item.Count);
		}
		Debug.Log ("Printing player potions");
		foreach (PotionItem item in playerPotionItems) {
			Debug.Log (item.Name);
			Debug.Log((item.Type).ToString());
			Debug.Log (item.Count);
		}
		Debug.Log ("Printing player story:");
		foreach (StoryItem item in playerStoryItems) {
			Debug.Log (item.Name);
			Debug.Log (item.Count);
		}
	}

	// Instantiate prefabs for items currently in inventory
	public void LoadPrefabs()
	{	
		Object ITEM;
		GameObject tempItem;
		foreach (WeaponItem item in playerWeaponItems) 
		{
			ITEM = Resources.Load("Prefab/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			activeObjects.Add(tempItem);
		}
		foreach (ArmorItem item in playerArmorItems) 
		{
			ITEM = Resources.Load("Prefab/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			activeObjects.Add(tempItem);
		}
		foreach (PotionItem item in playerPotionItems) 
		{
			ITEM = Resources.Load("Prefab/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			activeObjects.Add(tempItem);
		}
		foreach (StoryItem item in playerStoryItems) 
		{
			ITEM = Resources.Load("Prefab/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			activeObjects.Add(tempItem);
		}
		// Hide all objects
		foreach (GameObject item in activeObjects)
		{
			item.SetActive(false);
		}
	}

	// Calculate the rows and columns to display menu items and save as Vector3
	void SetMenuLocations()
	{
		Vector3 topLeft = GameObject.Find("MenuTopLeft").transform.position;
		Vector3 bottomRight = GameObject.Find("MenuBottomRight").transform.position;
		int columnNum = 4;
		int columnCounter = 0;

		float width = bottomRight.x - topLeft.x;
		float x = topLeft.x;
		float y = topLeft.y;

		float offset = width/(float)columnNum; 

		for (int i = 0; i < MAX_ITEMS; i++)
		{
			x += offset;

			Vector3 tempTransform = new Vector3(x, y);
			menuLocations.Add(tempTransform);

			if (columnCounter >= columnNum)
			{
				columnCounter = 0;
				x = topLeft.x;
				y += offset;
			}
		}
	}

	// Reset all locations of sprites in menu display
	public void UpdateDisplay()
	{
		for (int i = 0; i < activeObjects.Count; i++)
		{
			activeObjects[i].transform.position = menuLocations[i];
		}

	}

	// Show all objects in inventory
	public void Display()
	{
		displaying = true;
		foreach (GameObject item in activeObjects)
		{
			item.SetActive(true);
		}
	}

	public void Hide()
	{
		displaying = false;
		foreach (GameObject item in activeObjects)
		{
			item.SetActive(false);
		}
	}

	bool Displaying
	{
		get {return displaying;}
		set {displaying = value;}
	}

	public bool Battling
	{
		get {return battling;}
		set {battling = value;}
	}
}
