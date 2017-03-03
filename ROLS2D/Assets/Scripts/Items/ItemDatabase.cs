// NOTE: 3/1/17 Individual list types need to be created 
// In ReadItems() figure out type of item and add to appropriate list

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class ItemDatabase : MonoBehaviour 
{
	public TextAsset itemInventory; // xml file set in unity inspector
	public static List<BaseItem> itemList = new List<BaseItem>();

	private List<Dictionary<string, string>> itemDictionaryList = new List<Dictionary<string, string>>();
	private Dictionary<string, string> itemDictionary;


	void Awake()
	{
		// Read items from xml
		ReadItems();
		// Store items in item lists
		StoreItems();
	}


	// Read items from XML database
	public void ReadItems()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(itemInventory.text);
		XmlNodeList itemNodeList = xmlDocument.GetElementsByTagName("Item");

		// Grab item details and store in dictionary
		foreach (XmlNode itemInfo in itemNodeList)
		{
			XmlNodeList itemContent = itemInfo.ChildNodes;
			itemDictionary = new Dictionary<string, string>();

			foreach(XmlNode content in itemContent)
			{
				itemDictionary.Add(content.Name.ToString(), content.InnerText);
			}

			itemDictionaryList.Add(itemDictionary);
		}
	}

	void StoreItems()
	{
		// Add items to itemList database
		for (int i = 0; i < itemDictionaryList.Count; i++)
		{
			itemList.Add(new BaseItem(itemDictionaryList[i]));
		}
	}


	// Return a copy of the database item
	public BaseItem GetItem (string name)
	{
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemList[i].Name == name)
				return itemList[i];
		}
		return null;
	}


	// For debugging
	public void PrintItems()
	{
		foreach (BaseItem item in itemList)
		{
			Debug.Log(item.Name);
			Debug.Log(item.Count);
		}
	}
}
