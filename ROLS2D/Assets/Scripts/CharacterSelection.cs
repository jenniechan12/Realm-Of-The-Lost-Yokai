using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterSelection : MonoBehaviour {
	public Sprite[] characterSprites;
	public Image currentCharacter;
	int currentIndex = 0;
	// Use this for initialization
	void Start () {
		
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
	}

	public void RightButton(){
		currentIndex++;

		// Prevent Array out of Bound
		if (currentIndex >= characterSprites.Length) {
			currentIndex = characterSprites.Length - 1;
		}

		currentCharacter.overrideSprite = characterSprites [currentIndex];
	}
}
