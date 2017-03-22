using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class PuzzleDatabase : MonoBehaviour { 

	private List<Dictionary<string, string>> puzzleDictionaryList = new List<Dictionary<string, string>>();
	private Dictionary<string, string> puzzleDictionary = new Dictionary<string, string>();

	private TextAsset PUZZLE_DATABASE;
	string puzzleDatabase;


	void Awake()
	{
		puzzleDatabase = "XML/PuzzleDatabase";
	}

	// Use this for initialization
	void Start () {
		ReadItems(puzzleDatabase);
	}
	
	// Read puzzles from XML database
	public void ReadItems(string puzzleDatabaseXml)
	{
		puzzleDictionary.Clear();
		puzzleDictionaryList.Clear();

		PUZZLE_DATABASE = Resources.Load(puzzleDatabaseXml) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(PUZZLE_DATABASE.text);

		XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Puzzle");

		// Grab item details and store in dictionary
		foreach (XmlNode nodeInfo in nodeList)
		{
			XmlNodeList nodeContent = nodeInfo.ChildNodes;
			puzzleDictionary = new Dictionary<string, string>();

			foreach(XmlNode content in nodeContent)
			{
				puzzleDictionary.Add(content.Name.ToString(), content.InnerText);
			}

			puzzleDictionaryList.Add(puzzleDictionary);
		}
	}

	public List<Dictionary<string, string>> GetPuzzleList()
	{
		return puzzleDictionaryList;
	}

	// Return a dictionary of item
	public Dictionary<string, string> GetPuzzle (int number)
	{
		for (int i = 0; i < puzzleDictionaryList.Count; i++)
		{
			Dictionary<string, string> tempDict = puzzleDictionaryList[i];
			if (int.Parse(tempDict["Number"]) == number)
			{
				return tempDict;
			}
		}

		return null;
	}

	public string GetXml(int number)
	{
		for (int i = 0; i < puzzleDictionaryList.Count; i++)
		{
			Dictionary<string, string> tempDict = puzzleDictionaryList[i];
			if (int.Parse(tempDict["Number"]) == number)
			{
				return tempDict["Xml"];
			}
		}

		return null;
	}
}
