using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public class ClassDatabase : MonoBehaviour {
	public TextAsset characterClasses; // xml file for character classes
	public static List<BaseCharacterClass> classList = new List<BaseCharacterClass>();

	private List<Dictionary<string, string>> classDictionaryList = new List<Dictionary<string, string>> ();
	private Dictionary<string, string> classDictionary; 

	void Awake(){

		// Read character classes from xml
		ReadClasses();
		// Store character class in class lists
		StoreClasses();
	}


	// Read character classes from XML database
	public void ReadClasses(){
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (characterClasses.text);
		XmlNodeList classNodeList = xmlDocument.GetElementsByTagName ("Class");

		// Grab class info and store in dictionary
		foreach (XmlNode classInfo in classNodeList) {
			XmlNodeList classContent = classInfo.ChildNodes;
			classDictionary = new Dictionary<string, string> ();

			foreach (XmlNode content in classContent) {
				classDictionary.Add (content.Name.ToString (), content.InnerText);
			}

			classDictionaryList.Add (classDictionary);
		}
	}

	void StoreClasses(){
		
		//Add classes to classList database
		for (int i = 0; i < classDictionaryList.Count; i++) {
			classList.Add (new BaseCharacterClass (classDictionaryList [i]));
		}
	}

	//Return a copy of the database class
	public BaseCharacterClass GetClass (int index){
		if (index >= 0 && index < classList.Count)
			return classList [index];
		else
			Debug.Log ("Class List - Index is out of bound");
		return null;
	}

	// For debugging purpose
	public void PrintClasses(){
		foreach (BaseCharacterClass characterClass in classList) {
			Debug.Log (characterClass.ClassName);
			Debug.Log (characterClass.ClassDescription);
			Debug.Log (characterClass.Vitality);
			Debug.Log (characterClass.Strength);
			Debug.Log (characterClass.Defense);
			Debug.Log (characterClass.Luck);
		}
	}
}
