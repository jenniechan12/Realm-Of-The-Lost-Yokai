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

	private int strengthMod;
	private int defenseMod;
	private int vitalityMod;
	private int luckMod;


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
		UpdateStats();
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

		// Make sure our stat modifiers reflect the new equipment
		UpdateStats();
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

		// Make sure we update the stat modifiers
		UpdateStats();
	}


	// Update the stats struct to alter player stats
	void UpdateStats()
	{
		strengthMod = 0;
		defenseMod = 0;
		vitalityMod = 0;
		luckMod = 0;

		if (weapon != null)
		{
			strengthMod += weapon.Strength;
			defenseMod += weapon.Defense;
			vitalityMod += weapon.Vitality;
			luckMod += weapon.Luck;
		}
		if (armor != null)
		{
			strengthMod += armor.Strength;
			defenseMod += armor.Defense;
			vitalityMod += armor.Vitality;
			luckMod += armor.Luck;
		}
		if (boots != null)
		{
			strengthMod += boots.Strength;
			defenseMod += boots.Defense;
			vitalityMod += boots.Vitality;
			luckMod += boots.Luck;
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

	public int StrengthMod
	{
		get {return strengthMod;}
		set {strengthMod = value;}
	}
	public int DefenseMod
	{
		get {return defenseMod;}
		set {defenseMod = value;}
	}
	public int VitalityMod
	{
		get {return vitalityMod;}
		set {vitalityMod = value;}
	}
	public int LuckMod
	{
		get {return luckMod;}
		set {luckMod = value;}
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
