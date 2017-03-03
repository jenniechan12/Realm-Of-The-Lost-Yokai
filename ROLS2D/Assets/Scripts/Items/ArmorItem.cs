using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : BaseItem{

	private int defense;

	public ArmorItem(Dictionary<string, string> dictionary) : base(dictionary)
	{	

		defense = int.Parse(dictionary["ItemDefense"]);
	}

	int Defense
	{
		get {return defense;}
		set {defense = value;}
	}

}