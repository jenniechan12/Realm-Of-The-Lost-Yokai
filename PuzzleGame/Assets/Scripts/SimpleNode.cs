using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNode : MonoBehaviour {

	LevelManager levelManager;


	int maxCount;
	int currentCount;
	string type;
	int row;
	int column;

	void Awake()
	{
		type = "SIMPLE";
		maxCount = 0;
		currentCount = 0;
		row = 0;
		column = 0;
	}

	// Use this for initialization
	void Start () {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}
	

	public void SetupNode(int max, int r, int c)
	{
		maxCount = max;
		currentCount = max;
		row = r;
		column = c;
	}


	// Try to select the node
	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Cursor")
		{
			if (currentCount > 0)
			{
				if (levelManager.SelectNode(gameObject))
					currentCount--;
			}
		}
	}

	// Lock cursor on node
	public void OnTriggerStay2D(Collider2D other)
	{
		/*
		if (other.tag == "Cursor")
		{
			levelManager.SetCursor(gameObject);
			
		}
		*/
	}

	public void DecreaseCount()
	{
		if (currentCount > 0)
			currentCount--;
	}

	public void ResetCount()
	{
		currentCount = maxCount;
	}

	public string Type
	{
		get {return type;}
		set {type = value;}
	}

	public int Row
	{
		get {return row;}
		set {row = value;}
	}

	public int Column
	{
		get {return column;}
		set {column = value;}
	}

	public int CurrentCount
	{
		get {return currentCount; }
		set {currentCount = value; }
	}
}
