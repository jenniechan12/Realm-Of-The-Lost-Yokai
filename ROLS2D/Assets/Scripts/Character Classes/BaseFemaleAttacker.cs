using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFemaleAttacker : BaseCharacterClass {

	public BaseFemaleAttacker(){
		CharacterName = "Female Meelee";
		CharacterDescription = "Have high attack, average hp and defense, and terrible luck";
		Vitality = 1;
		Strength = 5;
		Agility = 1;
		Luck = 4;
	}
}
