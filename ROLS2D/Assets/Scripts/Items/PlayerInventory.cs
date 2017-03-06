// There are two lists, a list of weapon objects (player___Items) for storing information 
// and a list of the menu gameobjects (activeObjects). 
// They are linked by name. playerWeaponItem.Prefab == activeObject.name

// NOTE: 3/6 When transitioning to and from battling the menu open button needs to be turned on/off

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Xml;

public class PlayerInventory : MonoBehaviour
{

	private GameObject itemDatabaseObj;
	private ItemDatabase itemDatabase;
	private EquipManager equipManager;

	private List<GameObject> activeObjects = new List<GameObject>();
	private List<Vector3> menuLocations = new List<Vector3>();

	private GameObject selectedItem;

	private List<EquipItem> playerEquipItems = new List<EquipItem>();
	private List<ConsumableItem> playerConsumableItems = new List<ConsumableItem>();
	private List<StoryItem> playerStoryItems = new List<StoryItem> ();

	private TextAsset INVENTORY_XML;

	// Buttons
	private GameObject exitButton;
	private GameObject cancelItemButton;
	private GameObject equipItemButton;
	private GameObject unequipItemButton;
	private GameObject useItemButton;
	private GameObject giveItemButton;
	private GameObject openButton;
	private Text itemNameUI;
	private Text itemDescriptionUI;

	private int money;
	private const int MAX_MONEY = 100000;

	private const int MAX_ITEMS = 20;

	bool battling;	// Are we displaying in a battle or from world?
	bool displaying;

	// *********************************************************
	// AT START
	// *********************************************************
	void Awake()
	{
		battling = false;
		displaying = false;
		INVENTORY_XML = Resources.Load("XML/PlayerInventory") as TextAsset;
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

		equipManager = GameObject.Find("EquipManager").GetComponent<EquipManager>();

		SetButtons();
		
		itemNameUI = GameObject.Find("InventoryCanvas/ItemName").GetComponent<Text>();
		itemDescriptionUI = GameObject.Find("InventoryCanvas/ItemDescription").GetComponent<Text>();

		itemNameUI.text = "";
		itemDescriptionUI.text = "";
		// Load items from xml into inventory
		LoadInvetory();
		// Instantiate and set item prefab locations
		LoadPrefabs();
		SetMenuLocations();
		UpdateDisplay();
	}

	// Add functions for button clicks
	void SetButtons()
	{
		Button button;
		exitButton = GameObject.Find("ExitButton");
		if (exitButton)
		{
			button = exitButton.GetComponent<Button>();
			button.onClick.AddListener(ExitClick);
			exitButton.SetActive(false);
		}

		cancelItemButton = GameObject.Find("CancelItemButton");
		if (cancelItemButton)
		{
			button = cancelItemButton.GetComponent<Button>();
			button.onClick.AddListener(CancelItemClick);
			cancelItemButton.SetActive(false);
		}

		useItemButton = GameObject.Find("UseItemButton");
		if (useItemButton)
		{
			button = useItemButton.GetComponent<Button>();
			button.onClick.AddListener(UseItemClick);
			useItemButton.SetActive(false);
		}

		equipItemButton = GameObject.Find("EquipItemButton");
		if (equipItemButton)
		{
			button = equipItemButton.GetComponent<Button>();
			button.onClick.AddListener(EquipItemClick);
			equipItemButton.SetActive(false);
		}

		unequipItemButton = GameObject.Find("UnequipItemButton");
		if (unequipItemButton)
		{
			button = unequipItemButton.GetComponent<Button>();
			button.onClick.AddListener(UnequipItemClick);
			unequipItemButton.SetActive(false);
		}

		giveItemButton = GameObject.Find("GiveItemButton");
		if (giveItemButton)
		{
			button = giveItemButton.GetComponent<Button>();
			button.onClick.AddListener(GiveItemClick);
			giveItemButton.SetActive(false);
		}
		openButton = GameObject.Find("OpenButton");
		if (openButton)
		{
			button = openButton.GetComponent<Button>();
			button.onClick.AddListener(Display);
			if (!battling)
				openButton.SetActive(true);
			else
				openButton.SetActive(false);
		}
	}


	// Loads inventory from xml file
	void LoadInvetory ()
	{
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (INVENTORY_XML.text);
		XmlNodeList itemNodeList = xmlDocument.GetElementsByTagName ("Item");

		// Read items from xml, copy from database, and add to inventory
		foreach (XmlNode itemInfo in itemNodeList) 
		{
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

			// Get item dictionary, set count, and add item to list
			Dictionary<string, string> tempDict;

			if (itemDatabase != null) 
			{
				tempDict = itemDatabase.GetItem (tempName);
				if (tempDict != null) 
				{
					switch (tempDict ["ItemType"]) 
					{
						case "EQUIP":
							EquipItem  tempEquip = new EquipItem(tempDict);
							tempEquip.Count = int.Parse(tempCount);
							playerEquipItems.Add(tempEquip);
							break;
						case "CONSUMABLE":
							ConsumableItem  tempConsumable = new ConsumableItem(tempDict);
							tempConsumable.Count = int.Parse(tempCount);
							playerConsumableItems.Add(tempConsumable);
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


	// Instantiate prefabs for items currently in inventory
	public void LoadPrefabs()
	{	
		Object ITEM;
		GameObject tempItem;
		foreach (EquipItem item in playerEquipItems) 
		{
			ITEM = Resources.Load("Prefab/MenuItems/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			tempItem.transform.SetParent(GameObject.Find("InventoryCanvas").transform, true);
			activeObjects.Add(tempItem);
		}
		foreach (ConsumableItem item in playerConsumableItems) 
		{
			ITEM = Resources.Load("Prefab/MenuItems/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			tempItem.transform.SetParent(GameObject.Find("InventoryCanvas").transform, true);
			activeObjects.Add(tempItem);
		}
		foreach (StoryItem item in playerStoryItems) 
		{
			ITEM = Resources.Load("Prefab/MenuItems/" + item.Prefab) as GameObject;
			tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
			tempItem.name = item.Prefab;
			tempItem.transform.SetParent(GameObject.Find("InventoryCanvas").transform, true);
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
		Vector3 topLeft = GameObject.Find("InventoryCanvas/MenuTopLeft").transform.position;
		Vector3 bottomRight = GameObject.Find("InventoryCanvas/MenuBottomRight").transform.position;
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


	//************************************************************
	// ITEM FUNCTIONS
	//************************************************************
	// Add to or increase inventory count
	public void AddItem (string name, int count)
	{
		
		// Check to see if the player already has the item
		foreach (EquipItem item in playerEquipItems) {
			if (name == item.Name) {
				item.Count += count;
				return;
			}
		}
		foreach (ConsumableItem item in playerConsumableItems) {
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

		// Otherwhise create new item
		Dictionary<string, string> tempDict;
		tempDict = itemDatabase.GetItem (name);
		if (tempDict != null) 
		{
			Object ITEM;
			GameObject tempItem;
			switch (tempDict ["ItemType"]) 
			{
				case "EQUIP":
					// Add to inventory
					EquipItem tempEquip = new EquipItem (tempDict);
					tempEquip.Count = count;
					playerEquipItems.Add (tempEquip);

					// Add to menu active Object list
					ITEM = Resources.Load("Prefab/" + tempEquip.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					tempItem.name = tempEquip.Prefab;
					tempItem.SetActive(false);
					break;
				case "CONSUMABLE":
					// Add to inventory
					ConsumableItem tempConsumable = new ConsumableItem (tempDict);
					tempConsumable.Count = count;
					playerConsumableItems.Add (tempConsumable);

					// Add to menu active Object list
					ITEM = Resources.Load("Prefab/" + tempConsumable.Prefab) as GameObject;
					tempItem = Instantiate(ITEM, Vector3.zero, Quaternion.identity) as GameObject;
					activeObjects.Add(tempItem);
					tempItem.name = tempConsumable.Prefab;
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
		for (i = 0; i < playerEquipItems.Count; i++) 
		{
			if (playerEquipItems[i].Name == name)
			{
				playerEquipItems[i].Count -= count;
			}
		}
		for (i = 0; i < playerConsumableItems.Count; i++) 
		{
			if (playerConsumableItems[i].Name == name)
			{
				playerConsumableItems[i].Count -= count;
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
		if (i < playerEquipItems.Count) 
		{	// Make sure item exists
			if (playerEquipItems [i].Count <= 0) 
			{
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerEquipItems[i].Prefab == activeObjects[k].name)
					{
						GameObject tempObj = activeObjects[k];
						activeObjects.Remove(activeObjects[k]);
						Destroy(tempObj);
						UpdateDisplay();
						break;
					}
				}
				playerEquipItems.Remove (playerEquipItems [i]);
				return;
			}
		}
		if (i < playerConsumableItems.Count) 
		{	// Make sure item exists
			if (playerConsumableItems [i].Count <= 0) 
			{
				for (int k = 0; k < activeObjects.Count; k++)
				{
					if (playerConsumableItems[i].Prefab == activeObjects[k].name)
					{
						GameObject tempObj = activeObjects[k];
						activeObjects.Remove(activeObjects[k]);
						Destroy(tempObj);
						UpdateDisplay();
						break;
					}
				}
				playerConsumableItems.Remove (playerConsumableItems [i]);
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


	//************************************************************
	// DISPLAY FUNCTIONS
	//************************************************************

	// For debugging
	public void PrintInventory ()
	{	
		Debug.Log ("Printing player inventory:");

		Debug.Log ("Printing player equip items:");
		foreach (EquipItem item in playerEquipItems) {
			Debug.Log (item.Name);
			Debug.Log((item.Type).ToString());
			Debug.Log (item.Count);
		}
		Debug.Log ("Printing player consumable items:");
		foreach (ConsumableItem item in playerConsumableItems) {
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



	// Reset all locations of sprites in menu display
	// Call this after objects are removed or added to inventory
	public void UpdateDisplay()
	{
		for (int i = 0; i < activeObjects.Count; i++)
		{
			activeObjects[i].transform.position = menuLocations[i];
		}

	}

	// Show inventory UI and hide open button
	public void Display()
	{
		displaying = true;
		foreach (GameObject item in activeObjects)
		{
			item.SetActive(true);
		}
		exitButton.SetActive(true);
		openButton.SetActive(false);
	}

	// Hide inventory UI and show open button
	public void Hide()
	{
		if (!battling)
			openButton.SetActive(true);

		displaying = false;
		UnselectItem();
		foreach (GameObject item in activeObjects)
		{
			item.SetActive(false);
		}
		exitButton.SetActive(false);
	}



	//************************************************************
	// BUTTON FUNCTIONS
	//************************************************************

	// Exit the item inventory ui
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

	// Equip the currently selected items, change buttons
	public void EquipItemClick()
	{
		// Find the selected item in our item list and send name to equipManager
		foreach (EquipItem item in playerEquipItems) 
		{
			if (selectedItem.name == item.Prefab)
			{
				equipManager.EquipItem(item.Name);

				// Change buttons
				equipItemButton.SetActive(false);
				unequipItemButton.SetActive(true);
			}
		}
	}


	// Remove equipment, change buttons
	public void UnequipItemClick()
	{
		// Find the selected item in our item list and send name to equipManager
		foreach (EquipItem item in playerEquipItems) 
		{
			if (selectedItem.name == item.Prefab)
			{
				equipManager.UnequipItem(item.Name);

				// Change buttons
				equipItemButton.SetActive(true);
				unequipItemButton.SetActive(false);

			}
		}
	}


	// Unselect the selected item (remove select buttons)
	public void CancelItemClick()
	{
		UnselectItem();
	}

	public void GiveItemClick()
	{
	}


	// Player clicked an item in inventory, show selected item button options
	public void SelectItemClick(GameObject selectItem)
	{
		selectedItem = selectItem;

		foreach (EquipItem item in playerEquipItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				if (!battling)
				{
					itemNameUI.text = item.Name;
					itemDescriptionUI.text = item.Description;

					if (equipManager.IsEquipped(item.Name))
						unequipItemButton.SetActive(true);
					else
						equipItemButton.SetActive(true);

					cancelItemButton.SetActive(true);
					return;
				}
			}
		}
		foreach (ConsumableItem item in playerConsumableItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				if (battling)
				{
					itemNameUI.text = item.Name;
					itemDescriptionUI.text = item.Description;
					useItemButton.SetActive(true);
					giveItemButton.SetActive(true);
					cancelItemButton.SetActive(true);
					return;
				}
				else
				{
					itemNameUI.text = item.Name;
					itemDescriptionUI.text = item.Description;
					useItemButton.SetActive(true);
					cancelItemButton.SetActive(true);
					return;
				}
			}
		}
		foreach (StoryItem item in playerStoryItems) 
		{
			if (item.Prefab == selectedItem.name)
			{
				itemNameUI.text = item.Name;
				itemDescriptionUI.text = item.Description;
				useItemButton.SetActive(true);
				giveItemButton.SetActive(true);
				cancelItemButton.SetActive(true);
				return;
			}
		}

	}

	// Item was unselected, turn off selected item buttons
	public void UnselectItem()
	{
		if (selectedItem)
			selectedItem = null;

		// Turn off selected item buttons
		itemNameUI.text = "";
		itemDescriptionUI.text = "";
		equipItemButton.SetActive(false);
		unequipItemButton.SetActive(false);
		giveItemButton.SetActive(false);
		cancelItemButton.SetActive(false);
		useItemButton.SetActive(false);

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
}
