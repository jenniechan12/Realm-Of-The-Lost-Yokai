using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterSelection : MonoBehaviour {
	public Sprite[] characterSprites;
	public Transform[] attributes;
	public Image currentCharacter;

	// List of Classes
	private BaseCharacterClass class1 = new BaseMaleAttacker(); 
	private BaseCharacterClass class2 = new BaseFemaleAttacker();
	private BaseCharacterClass class3 = new BaseMaleHeal(); 
	private BaseCharacterClass class4 = new BaseFemaleHeal();
	private BaseCharacterClass class5 = new BaseMaleThief(); 
	private BaseCharacterClass class6 = new BaseFemaleThief();
	private BaseCharacterClass class7 = new BaseMaleLuck(); 
	private BaseCharacterClass class8 = new BaseFemaleLuck();
	 
	private BaseCharacterClass[] characterClasses;
	private BaseCharacterClass currentClass; 


	int currentIndex = 0;
	// Use this for initialization
	void Start () {
		characterClasses = new BaseCharacterClass[8]{
			class1, class2, class3, class4, class5, class6, class7, class8
		};

		currentClass = characterClasses [currentIndex];
		DisplayStat ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Display Characters
	public void LeftButton(){
		currentIndex--;

		// Prevent Array out of bound
		if (currentIndex < 0) {
			currentIndex = 0;
		}

		currentCharacter.overrideSprite = characterSprites [currentIndex];
		currentClass = characterClasses [currentIndex];
		DisplayStat ();
	}

	public void RightButton(){
		currentIndex++;

		// Prevent Array out of Bound
		if (currentIndex >= characterSprites.Length) {
			currentIndex = characterSprites.Length - 1;
		}

		currentCharacter.overrideSprite = characterSprites [currentIndex];
		currentClass = characterClasses [currentIndex];
		DisplayStat ();
	}

	private void DisplayStat(){
		attributes [0].localScale = new Vector3 (currentClass.Vitality, 1, 1);
		attributes [1].localScale = new Vector3 (currentClass.Strength, 1, 1);
		attributes [2].localScale = new Vector3 (currentClass.Agility, 1, 1); 
		attributes [3].localScale = new Vector3 (currentClass.Luck, 1, 1);
	}
}
