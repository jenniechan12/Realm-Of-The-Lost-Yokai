using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterClass{

	// Basic Character Info
	private string characterName; 
	private string characterDescription;

	// Basic Character Stats
	private int vitality;	// Vitality - influence in HP or health
	private int strength;	// Strength - influence in Attack and Damage 
	private int agility;	// Agility - influence in Accuracy and Evasion 
	private int luck; 		// Luck - influence in Crit and Flee 

	// TODO: Basic Character Spiritual Animal w/ specializes attack 

	public string CharacterName{
		get{ return characterName; }
		set{ characterName = value; } 
	}

	public string CharacterDescription{
		get{ return characterDescription; }
		set{ characterDescription = value; }
	}

	public int Vitality{
		get{ return vitality;}
		set{ vitality = value;}
	}

	public int Strength{
		get{ return strength;}
		set{ strength = value;}
	}

	public int Agility{
		get{ return agility;}
		set{ agility = value;}
	}

	public int Luck{
		get{ return luck;}
		set{ luck = value;}
	}

}
