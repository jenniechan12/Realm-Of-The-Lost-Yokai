using UnityEngine;
using System.Collections;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SaveManager : MonoBehaviour
{
	private TextAsset LevelCompletionDatabase;
	private XmlDocument xmlDocument; 
	private string pathString; 

	void Awake(){
		pathString = "XML/LevelCompletionDatabase";
		LevelCompletionDatabase = Resources.Load (pathString) as TextAsset;
	}

	// Use this for initialization
	void Start ()
	{
		LoadFile ();
		SaveFile (3);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
		
	public void LoadFile(){
		Debug.Log ("Starting to Load File");
		xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (LevelCompletionDatabase.text);

		XmlNode root = xmlDocument.FirstChild;
		foreach (XmlNode content in root.ChildNodes) {
			Debug.Log (content.Name);
		}
		Debug.Log ("Finish Loading File Now!");
	}

	public void SaveFile(int level){
		Debug.Log ("Starting to Save File");
		xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (LevelCompletionDatabase.text);

		XmlNode root = xmlDocument.FirstChild;
		foreach (XmlNode content in root.ChildNodes) {
			if (content.Name == "Level" + level.ToString ()) {
				content.InnerText = "Yes";
				break;
			}
		}

		xmlDocument.Save (Application.dataPath.ToString () + "/Resources/" + pathString + ".xml");
		Debug.Log ("Finish Saving File Now!");

	}
}

