using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterSelection : MonoBehaviour {
	public Sprite[] characterSprites;
	public Transform[] attributes;
	public Image currentCharacter;

	// List of Classes
	private GameObject classDatabaseObj; 
	private ClassDatabase classDatabase; 

	private BaseCharacterClass currentClass; 
	public BaseCharacterClass selectClass;

	int currentIndex = 0;

	// Use this for initialization
	void Start () {
		
		// References to objects and componenets 	
		classDatabaseObj = GameObject.Find("CharacterClassManager");
		if (classDatabaseObj)
			classDatabase = classDatabaseObj.GetComponent<ClassDatabase> ();
		if (classDatabaseObj == null)
			Debug.Log ("Cannot find class database object in character selection scene");
		if (classDatabase == null) {
			Debug.Log ("Cannot find class database component in character selection scene");
		}

		currentClass = classDatabase.GetClass(currentIndex);
		DisplayStat ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Left Button Function - Display Previous Character
	public void LeftButton(){
		currentIndex--;

		// Prevent Array out of bound
		if (currentIndex < 0) {
			currentIndex = 0;
		}

		currentCharacter.overrideSprite = characterSprites [currentIndex];
		currentClass = classDatabase.GetClass(currentIndex);		
		DisplayStat ();
	}

	// Right Button Function - Display Next Character
	public void RightButton(){
		currentIndex++;

		// Prevent Array out of Bound
		if (currentIndex >= characterSprites.Length) {
			currentIndex = characterSprites.Length - 1;
		}

		currentCharacter.overrideSprite = characterSprites [currentIndex];
		currentClass = classDatabase.GetClass(currentIndex);
		DisplayStat ();
	}

	// Confirm Button Function - Confirm Player's Character Selection
	public void SelectCharacter(){
		selectClass = currentClass;
		Debug.Log (currentClass.ClassName);
	}

	private void DisplayStat(){
		attributes [0].localScale = new Vector3 (currentClass.Vitality, 1, 1);
		attributes [1].localScale = new Vector3 (currentClass.Strength, 1, 1);
		attributes [2].localScale = new Vector3 (currentClass.Defense, 1, 1); 
		attributes [3].localScale = new Vector3 (currentClass.Luck, 1, 1);
	}
}
