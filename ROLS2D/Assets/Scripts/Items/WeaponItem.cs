using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : BaseItem{

	private int strength;

	public WeaponItem(Dictionary<string, string> dictionary)
	{	
		strength = int.Parse(dictionary["ItemStrength"]);
	}

	int Strength
	{
		get {return strength;}
		set {strength = value;}
	}

}
