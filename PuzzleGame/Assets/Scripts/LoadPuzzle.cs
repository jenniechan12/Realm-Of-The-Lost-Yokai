using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class LoadPuzzle : MonoBehaviour {

	private TextAsset PUZZLE_XML; 

	private List<Dictionary<string, string>> nodeDictionaryList = new List<Dictionary<string, string>>();
	private Dictionary<string, string> puzzleInfoDictionary = new Dictionary<string, string>();

	void Awake()
	{
		
	}

	// Read items from XML database
	public void ReadItems(string puzzleLocation)
	{
		Dictionary<string, string> nodeDictionary = new Dictionary<string, string>();
		puzzleInfoDictionary.Clear();
		nodeDictionaryList.Clear();

		PUZZLE_XML = Resources.Load(puzzleLocation) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(PUZZLE_XML.text);

		XmlNodeList nodeNodeList = xmlDocument.GetElementsByTagName("PuzzleInfo");

		// Grab item details and store in dictionary
		foreach (XmlNode nodeInfo in nodeNodeList)
		{
			XmlNodeList nodeContent = nodeInfo.ChildNodes;
			puzzleInfoDictionary = new Dictionary<string, string>();

			foreach(XmlNode content in nodeContent)
			{
				puzzleInfoDictionary.Add(content.Name.ToString(), content.InnerText);
			}
		}

		nodeNodeList = xmlDocument.GetElementsByTagName("Node");

		// Grab item details and store in dictionary
		foreach (XmlNode nodeInfo in nodeNodeList)
		{
			XmlNodeList nodeContent = nodeInfo.ChildNodes;
			nodeDictionary = new Dictionary<string, string>();

			foreach(XmlNode content in nodeContent)
			{
				nodeDictionary.Add(content.Name.ToString(), content.InnerText);
			}

			nodeDictionaryList.Add(nodeDictionary);
		}
	}

	public List<Dictionary<string, string>> GetPuzzleNodes()
	{
		return nodeDictionaryList;
	}

	public Dictionary<string, string> GetPuzzleInfo()
	{
		return puzzleInfoDictionary;
	}

	/*
	// Return a dictionary of item
	public Dictionary<string, string> GetItem (string name)
	{
		for (int i = 0; i < NodeDictionaryList.Count; i++)
		{
			Dictionary<string, string> tempDict = NodeDictionaryList[i];
			if (tempDict["ItemName"] == name)
			{
				return tempDict;
			}
		}

		return null;
	}
	*/

}
