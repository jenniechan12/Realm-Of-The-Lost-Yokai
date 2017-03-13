using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNode : MonoBehaviour {

	LevelManager levelManager;


	int maxCount;
	int currentCount;
	string type;
	int gridLocation;

	void Awake()
	{
		type = "SIMPLE";
		maxCount = 0;
		currentCount = 0;
	}

	// Use this for initialization
	void Start () {
		levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}



	public void SetupNode(int max, int location)
	{
		maxCount = max;
		currentCount = max;
		gridLocation = location;
	}


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

	public void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Cursor")
		{
			levelManager.SetCursor(gameObject.transform.position);
			
		}
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

	public int GridLocation
	{
		get {return gridLocation;}
		set {gridLocation = value;}
	}

	public int CurrentCount
	{
		get {return currentCount; }
		set {currentCount = value; }
	}
}
