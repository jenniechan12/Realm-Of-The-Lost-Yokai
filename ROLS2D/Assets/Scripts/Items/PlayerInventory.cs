// There are two lists, a list of weapon objects (player___Items) for storing information 
// and a list of the menu gameobjects (activeObjects). 
// They are linked by name. playerWeaponItem.Prefab == activeObject.name

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

	private GameObject selectedItem;

	private List<WeaponItem> playerWeaponItems = new List<WeaponItem> ();
	private List<ArmorItem> playerArmorItems = new List<ArmorItem> ();
	private List<PotionItem> playerPotionItems = new List<PotionItem> ();
	private List<StoryItem> playerStoryItems = new List<StoryItem> ();

	public TextAsset inventoryText;

	// Buttons
	private GameObject exitButton;
	private GameObject cancelItemButton;
	private GameObject equipItemButton;
	private GameObject useItemButton;
	private GameObject giveItemButton;

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

		exitButton = GameObject.Find("ExitButton");
		if (exitButton)
			exitButton.SetActive(false);

		cancelItemButton = GameObject.Find("CancelItemButton");
		if (cancelItemButton)
			cancelItemButton.SetActive(false);

		useItemButton = GameObject.Find("UseItemButton");
		if (useItemButton)
			useItemButton.SetActive(false);

		equipItemButton = GameObject.Find("EquipItemButton");
		if (equipItemButton)
				equipItemButton.SetActive(false);

		giveItemButton = GameObject.Find("GiveItemButton");
		if (giveItemButton)
			giveItemButton.SetActive(false);

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
					// Add to inventory
					WeaponItem tempWeapon = new WeaponItem (tempDict);
					tempWeapon.Count = count;
					playerWeaponItems.Add (tempWeapon);
					// Add to menu active Object list
					ITEM = Resources.Load("Prefab/" + tempWeapon.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					tempItem.name = tempWeapon.Prefab;
					tempItem.SetActive(false);
					break;
				case "ARMOR":
					// Add to inventory
					ArmorItem tempArmor = new ArmorItem (tempDict);
					tempArmor.Count = count;
					playerArmorItems.Add (tempArmor);
					// Add to menu active Object list
					ITEM = Resources.Load("Prefab/" + tempArmor.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					tempItem.name = tempArmor.Prefab;
					tempItem.SetActive(false);
					break;
				case "POTION":
					// Add to inventory
					PotionItem tempPotion = new PotionItem (tempDict);
					tempPotion.Count = count;
					playerPotionItems.Add (tempPotion);
					// Add to menu active Object list
					ITEM = Resources.Load("Prefab/" + tempPotion.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					tempItem.name = tempPotion.Prefab;
					tempItem.SetActive(false);
					break;
				case "STORY":
					// Add to inventory
					StoryItem tempStory = new StoryItem (tempDict);
					tempStory.Count = count;
					playerStoryItems.Add (tempStory);
					// Add to menu active Object list
					ITEM = Resources.Load("Prefab/" + tempStory.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					tempItem.name = tempStory.Prefab;
					tempItem.SetActive(false);
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
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerWeaponItems[i].Prefab == activeObjects[k].name)
					{
						GameObject tempObj = activeObjects[k];
						activeObjects.Remove(activeObjects[k]);
						Destroy(tempObj);
						UpdateDisplay();
						break;
					}
				}
				playerWeaponItems.Remove (playerWeaponItems [i]);
				return;
			}
		}
		if (i < playerArmorItems.Count) 
		{	// Make sure item exists
			if (playerArmorItems [i].Count <= 0) 
			{
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerArmorItems[i].Prefab == activeObjects[k].name)
					{
						GameObject tempObj = activeObjects[k];
						activeObjects.Remove(activeObjects[k]);
						Destroy(tempObj);
						UpdateDisplay();
						break;
					}
				}
				playerArmorItems.Remove (playerArmorItems [i]);
				return;
			}
		}
		if (i < playerPotionItems.Count) 
		{	// Make sure item exists
			if (playerPotionItems [i].Count <= 0) 
			{
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerPotionItems[i].Prefab == activeObjects[k].name)
					{
						GameObject tempObj = activeObjects[k];
						activeObjects.Remove(activeObjects[k]);
						Destroy(tempObj);
						UpdateDisplay();
						break;
					}
				}
				playerPotionItems.Remove (playerPotionItems [i]);
				return;
			}
		}
		if (i < playerStoryItems.Count) 
		{	// Make sure item exists
			if (playerStoryItems [i].Count <= 0) 
			{
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerStoryItems[i].Prefab == activeObjects[k].name)
					{
						GameObject tempObj = activeObjects[k];
						activeObjects.Remove(activeObjects[k]);
						Destroy(tempObj);
						UpdateDisplay();
						break;
					}
				}
				playerStoryItems.Remove (playerStoryItems [i]);
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

	//************************************************************
	// DISPLAY FUNCTIONS
	//************************************************************

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
			columnCounter++;
			x += offset;

			Vector3 tempTransform = new Vector3(x, y);
			menuLocations.Add(tempTransform);

			if (columnCounter >= columnNum)
			{
				columnCounter = 0;
				x = topLeft.x;
				y -= offset;
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
		exitButton.SetActive(true);
	}

	public void Hide()
	{
		displaying = false;
		foreach (GameObject item in activeObjects)
		{
			item.SetActive(false);
		}
		exitButton.SetActive(false);
	}

	//************************************************************
	// RETRIEVAL FUNCTIONS
	//************************************************************

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


	//************************************************************
	// BUTTON FUNCTIONS
	//************************************************************

	public void ExitClick()
	{
		Hide();
		if (battling)
		{
			BattleManager battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
			if (battleManager)
				battleManager.UnhideButtons();
		}
		
	}

	public void UseItemClick()
	{
		// Call effect of selected item
	}

	public void EquipItemClick()
	{
		// Change equipment
	}

	public void CancelItemClick()
	{
		UnselectItem();
	}

	public void GiveItemClick()
	{
	}


	public void SelectItem(GameObject selectItem)
	{
		selectedItem = selectItem;

		foreach (WeaponItem item in playerWeaponItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				if (!battling)
				{
					equipItemButton.SetActive(true);
					cancelItemButton.SetActive(true);
					return;
				}
			}
		}
		foreach (ArmorItem item in playerArmorItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				if (!battling)
				{
					equipItemButton.SetActive(true);
					cancelItemButton.SetActive(true);
					return;
				}
			}
		}
		foreach (PotionItem item in playerPotionItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				useItemButton.SetActive(true);
				giveItemButton.SetActive(true);
				cancelItemButton.SetActive(true);
				return;
			}
		}
		foreach (StoryItem item in playerStoryItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				useItemButton.SetActive(true);
				giveItemButton.SetActive(true);
				cancelItemButton.SetActive(true);
				return;
			}
		}

	}

	public void UnselectItem()
	{
		if (selectedItem)
			selectedItem = null;

		equipItemButton.SetActive(false);
		giveItemButton.SetActive(false);
		cancelItemButton.SetActive(false);
		useItemButton.SetActive(false);
	}
}
