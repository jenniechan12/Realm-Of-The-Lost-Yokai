using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : BaseItem{

	private string stat;
	private int effect;

	public PotionItem(Dictionary<string, string> dictionary) : base(dictionary)
	{	
		stat = dictionary["ItemStat"];
		effect = int.Parse(dictionary["ItemEffect"]);
	}

	int Effect
	{
		get {return effect;}
		set {effect = value;}
	}

	string Stat
	{
		get {return stat;}
		set {stat = value;}
	}

}
