using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class EquipManager : MonoBehaviour {

	private TextAsset INVENTORY_XML;
	private EquipItem weapon;
	private EquipItem armor;
	private EquipItem boots;
	private ItemDatabase itemDatabase;

	void Awake()
	{
		INVENTORY_XML = Resources.Load("XML/PlayerInventory") as TextAsset;
		weapon = null;
		armor = null;
		boots = null;
	}

	// Use this for initialization
	void Start () {
		itemDatabase = GameObject.Find("ItemDatabase").GetComponent<ItemDatabase>();
		LoadEquipment();
		Print();
	}


	// Equip the item to the appropriate slot
	public void EquipItem(string name)
	{
		// Get the item dictionary from the database
		Dictionary<string, string> tempDict;
		tempDict = itemDatabase.GetItem(name);

		// Figure out what type of item it is and equip
		if (tempDict != null) 
		{
			switch (tempDict ["ItemSubtype"]) 
			{
				case "WEAPON":
					EquipItem  tempEquip = new EquipItem(tempDict);
					weapon = tempEquip;
					break;
				case "ARMOR":
					EquipItem tempArmor = new EquipItem(tempDict);
					armor = tempArmor;
					break;
				case "BOOTS":
					EquipItem tempBoots = new EquipItem(tempDict);
					boots = tempBoots;
					break;
			}
		}
	}


	// Unequip the item from the appropriate slot
	public void UnequipItem(string name)
	{
		// Get item dictionary from database
		Dictionary<string, string> tempDict;
		tempDict = itemDatabase.GetItem(name);

		// Figure out what kind of item it is and equip the appropriate slot
		if (tempDict != null) 
		{
			switch (tempDict ["ItemSubtype"]) 
			{
				case "WEAPON":
					weapon = null;
					break;
				case "ARMOR":
					armor = null;
					break;
				case "BOOTS":
					boots = null;
					break;
			}
		}
	}

	// Is the time currently 
	public bool IsEquipped(string name)
	{
		if (weapon != null)
		{
			if (weapon.Name == name)
			{
				return true;
			}
		}
		if (armor != null)
		{
			if (armor.Name == name)
			{
				return true;
			}
		}
		if (boots != null)
		{
			if (boots.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	// Load equipment from PlayerInventory.xml
	void LoadEquipment()
	{
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (INVENTORY_XML.text);
		XmlNodeList itemNode = xmlDocument.GetElementsByTagName ("EquipedWeapon");

		// Read items from xml, copy from database, and add to inventory
		foreach (XmlNode itemInfo in itemNode) 
		{
			string tempName = "";	
			tempName = itemInfo.InnerText;
				
			Dictionary<string, string> tempDict;
			// Copy item from database and set count
			if (itemDatabase != null) 
			{
				tempDict = itemDatabase.GetItem (tempName);
				if (tempDict != null) 
				{
					switch (tempDict ["ItemSubtype"]) 
					{
						case "WEAPON":
							EquipItem  tempEquip = new EquipItem(tempDict);
							weapon = tempEquip;
							break;
						case "NA":
							weapon = null;
							break;
					}
				}
			}
		}

		itemNode = xmlDocument.GetElementsByTagName ("EquipedArmor");

		// Read items from xml, copy from database, and add to inventory
		foreach (XmlNode itemInfo in itemNode) 
		{
			string tempName = "";	
			tempName = itemInfo.InnerText;
				
			Dictionary<string, string> tempDict;
			// Copy item from database and set count
			if (itemDatabase != null) 
			{
				tempDict = itemDatabase.GetItem (tempName);
				if (tempDict != null) 
				{
					switch (tempDict ["ItemSubtype"]) 
					{
						case "ARMOR":
							EquipItem  tempArmor = new EquipItem(tempDict);
							armor = tempArmor;
							break;
						case "NA":
							armor = null;
							break;
					}
				}
			}
		}

		itemNode = xmlDocument.GetElementsByTagName ("EquipedBoots");

		// Read items from xml, copy from database, and add to inventory
		foreach (XmlNode itemInfo in itemNode) 
		{
			string tempName = "";	
			tempName = itemInfo.InnerText;

			Dictionary<string, string> tempDict;
			// Copy item from database and set count
			if (itemDatabase != null) 
			{
				tempDict = itemDatabase.GetItem (tempName);
				if (tempDict != null) 
				{
					switch (tempDict ["ItemSubtype"]) 
					{
						case "NA":
							boots = null;
							break;
						case "BOOTS":
							EquipItem tempBoots = new EquipItem (tempDict);
							boots = tempBoots;
							break;
					}
				}
			}
		}
	}

	// For debugging
	public void Print()
	{
		Debug.Log("Printing equipped items:");
		if (weapon != null)
		{
			Debug.Log(weapon.Name);
		}
		else 	
			Debug.Log("No weapon equipped.");
		if (armor != null)
		{
			Debug.Log(armor.Name);
		}
		else 	
			Debug.Log("No armor equipped.");
		if (boots != null)
		{
			Debug.Log(boots.Name);
		}
		else 	
			Debug.Log("No boots equipped.");
	}
}
