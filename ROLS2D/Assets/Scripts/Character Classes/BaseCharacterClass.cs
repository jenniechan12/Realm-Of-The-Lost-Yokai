using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterClass{

	// Basic Character Info
	protected string className; 
	protected string classDescription;

	// Basic Character Stats
	protected int vitality;	// Vitality - influence in HP or health
	protected int strength;	// Strength - influence in Attack and Damage 
	protected int defense;	// Defense - influence in Defense 
	protected int luck; 	// Luck - influence in Crit and Dodge/Evasion 

	// TODO: Basic Character Spiritual Animal w/ specializes attack 

	// Constructor for Character Class Database
	public BaseCharacterClass(Dictionary<string, string> dictionary){
		// Set value from dictionary
		className = dictionary["ClassName"];
		classDescription = dictionary ["ClassDescription"];
		vitality = int.Parse (dictionary ["Vitality"]);
		strength = int.Parse (dictionary ["Strength"]);
		defense = int.Parse (dictionary ["Defense"]);
		luck = int.Parse (dictionary ["Luck"]);
	}

	// Default Constructor
	public BaseCharacterClass(){
		// Set default values
		className = "";
		classDescription = "";
		vitality = 0;
		strength = 0;
		defense = 0;
		luck = 0;
	}

	// Copy Constructor
	public BaseCharacterClass(BaseCharacterClass characterClass){
		// Set default values
		className = characterClass.ClassName;
		classDescription = characterClass.ClassDescription;
		vitality = characterClass.Vitality;
		strength = characterClass.Strength;
		defense = characterClass.Defense;
		luck = characterClass.Luck;
	}

	// Get and Set Function
	public string ClassName{
		get{ return className; }
		set{ className = value; } 
	}

	public string ClassDescription{
		get{ return classDescription; }
		set{ classDescription = value; }
	}

	public int Vitality{
		get{ return vitality;}
		set{ vitality = value;}
	}

	public int Strength{
		get{ return strength;}
		set{ strength = value;}
	}

	public int Defense{
		get{ return defense;}
		set{ defense = value;}
	}

	public int Luck{
		get{ return luck;}
		set{ luck = value;}
	}

}
