// NOTE: 3/1/17 Individual list types need to be created 
// In ReadItems() figure out type of item and add to appropriate list

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class ItemDatabase : MonoBehaviour 
{
	private TextAsset ITEM_LIST_XML; // xml file set in unity inspector

	private List<Dictionary<string, string>> itemDictionaryList = new List<Dictionary<string, string>>();
	private Dictionary<string, string> itemDictionary;


	void Awake()
	{
		// Read items from xml
		ITEM_LIST_XML = Resources.Load("XML/ItemList") as TextAsset;
		ReadItems();
	}


	// Read items from XML database
	public void ReadItems()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(ITEM_LIST_XML.text);
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

	// Return a dictionary of item
	public Dictionary<string, string> GetItem (string name)
	{
		for (int i = 0; i < itemDictionaryList.Count; i++)
		{
			Dictionary<string, string> tempDict = itemDictionaryList[i];
			if (tempDict["ItemName"] == name)
			{
				return tempDict;
			}
		}

		return null;
	}
}
