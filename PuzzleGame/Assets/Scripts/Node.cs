using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	LevelManager levelManager;
	SpriteRenderer spriteRenderer;

	int maxCount;
	int currentCount;
	string leaveType;
	string moveType;
	string specialType;
	string countType;
	int row;
	int column;
	enum HitColor {BLACK, BLUE, PURPLE, RED};
	HitColor currentColor;

	void Awake()
	{
		leaveType = "SIMPLE";
		moveType = "STATIC";
		specialType = "NONE";
		countType = "NORMAL";
		maxCount = 0;
		currentCount = 0;
		row = 0;
		column = 0;
		currentColor = HitColor.BLACK;
	}

	// Use this for initialization
	void Start () {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		if (leaveType != "EMPTY")
			spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		else
			spriteRenderer = null;
		UpdateColor();
	}
	

	public void SetupNode(int max, int r, int c, string lT, string mT, string sT, string cT)
	{
		maxCount = max;
		currentCount = max;
		row = r;
		column = c;
		leaveType = lT;
		moveType = mT;
		specialType = sT;
		countType = cT;
	}


	/*
	// Try to select the node
	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Cursor")
		{
			if (currentCount > 0)
			{
				levelManager.SelectNode(gameObject);
			}
		}
	}*/

	private void UpdateColor()
	{
		if (leaveType == "EMPTY")
		{
			return;
		}
		if (currentCount == 3)
		{
			spriteRenderer.color = Color.red;
			currentColor = HitColor.RED;
		}
		else if (currentCount == 2)
		{
			spriteRenderer.color = Color.magenta;
			currentColor = HitColor.PURPLE;
		}
		else if (currentCount == 1)
		{
			spriteRenderer.color = Color.blue;
			currentColor = HitColor.BLUE;
		}
		else
		{
			spriteRenderer.color = Color.black;
			currentColor = HitColor.BLACK;
		}
	}


	public void DecreaseCount()
	{
		if (currentCount > 0)
		{
			currentCount--;
			UpdateColor();
		}
	}

	public void Reset()
	{
		currentCount = maxCount;
		UpdateColor();
	}

	public void Undo()
	{
		currentCount++;
		UpdateColor();
	}

	public string CountType
	{
		get {return countType;}
		set {countType = value;}
	}

	public string LeaveType
	{
		get {return leaveType;}
		set {leaveType = value;}
	}

	public string MoveType
	{
		get {return moveType;}
		set {moveType = value;}
	}

	public string SpecialType
	{
		get {return specialType;}
		set {specialType = value;}
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