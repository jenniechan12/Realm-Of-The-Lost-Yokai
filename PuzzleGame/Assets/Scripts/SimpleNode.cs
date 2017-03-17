using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNode : MonoBehaviour {

	LevelManager levelManager;
	SpriteRenderer spriteRenderer;

	int maxCount;
	int currentCount;
	string type;
	int row;
	int column;
	enum HitColor {BLACK, BLUE, PURPLE, RED};
	HitColor currentColor;

	void Awake()
	{
		type = "SIMPLE";
		maxCount = 0;
		currentCount = 0;
		row = 0;
		column = 0;
		currentColor = HitColor.BLACK;
	}

	// Use this for initialization
	void Start () {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
			Debug.Log("Error finding sprite renderer in simple node.");
		UpdateColor();
	}
	

	public void SetupNode(int max, int r, int c, string t)
	{
		maxCount = max;
		currentCount = max;
		row = r;
		column = c;
		type = t;
	}


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
	}

	private void UpdateColor()
	{
		if (spriteRenderer == null)
			Debug.Log("No sprite renderer.");
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
