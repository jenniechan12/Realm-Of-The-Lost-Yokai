using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LoadPuzzleManager : MonoBehaviour {
	
	private TextAsset _puzzleXML;
	private List<Dictionary<string,string>> _nodeDictionaryList = new List<Dictionary<string, string>> ();
	private Dictionary<string,string> _nodeDictionary = new Dictionary<string, string> ();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void LoadPuzzle(string _puzzlePath){

		// Clear/Reset the List and Dictionary
		_nodeDictionary.Clear();
		_nodeDictionaryList.Clear ();

		// Loading Puzzle XML file
		Debug.Log ("Starting to Load Puzzle File");
		_puzzleXML = Resources.Load (_puzzlePath) as TextAsset;
		XmlDocument _xmlDocument = new XmlDocument ();
		_xmlDocument.LoadXml (_puzzleXML.text);


		// Grab node info and store in a dictionary
		XmlNodeList _nodeList = _xmlDocument.GetElementsByTagName("Node");

		foreach (XmlNode _nodeInfo in _nodeList) {
			XmlNodeList _nodeContent = _nodeInfo.ChildNodes;
			_nodeDictionary = new Dictionary<string,string> ();

			foreach (XmlNode _content in _nodeContent) {
				_nodeDictionary.Add (_content.Name.ToString (), _content.InnerText);
			}

			_nodeDictionaryList.Add (_nodeDictionary);
		}
	}

	public List<Dictionary<string, string>> GetPuzzleNodes(){
		//DisplayPuzzleInfo ();
		return _nodeDictionaryList;
	}

	// For Debugging Purposes
	private void DisplayPuzzleInfo(){
		foreach (Dictionary<string,string> _info in _nodeDictionaryList) {
			Debug.Log (_info.Count);
		}
	}

}
