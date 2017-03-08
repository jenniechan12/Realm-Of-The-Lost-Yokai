using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : BaseItem {

	private int strength;
	private int defense;
	private int luck;
	private int vitality;

	public EquipItem(Dictionary<string, string> dictionary) : base(dictionary)
	{	

		strength = int.Parse(dictionary["ItemStrength"]);
		defense = int.Parse(dictionary["ItemDefense"]);
		luck = int.Parse(dictionary["ItemLuck"]);
		vitality = int.Parse(dictionary["ItemVitality"]);
	}

	public int Strength
	{
		get {return strength;}
		set {strength = value;}
	}

	public int Defense
	{
		get {return defense;}
		set {defense = value;}
	}

	public int Luck
	{
		get {return luck;}
		set {luck = value;}
	}

	public int Vitality
	{
		get {return vitality;}
		set {vitality = value;}
	}
}
