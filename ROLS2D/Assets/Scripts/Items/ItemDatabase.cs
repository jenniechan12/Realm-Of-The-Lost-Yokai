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
		ReadItems();
		for (int i = 0; i < itemDictionaryList.Count; i++)
		{
			itemList.Add(new BaseItem(itemDictionaryList[i]));
		}
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

	// Return a copy of the database item
	public BaseItem CopyItem (string name)
	{
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemList[i].Name == name)
				return itemList[i];
		}
		return null;
	}

	public void PrintItems()
	{
		foreach (BaseItem item in itemList)
		{
			Debug.Log(item.Name);
			Debug.Log(item.Count);
		}
	}
}
