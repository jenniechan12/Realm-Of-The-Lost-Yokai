// Manages temp line and line 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineManager : MonoBehaviour {

	LevelManager levelManager;

	Button clearButton;
	Button undoButton;

	Object LINE;
	Object TEMP_LINE;
	GameObject lineObject;	// Line as nodes are selected
	GameObject tempLineObject;	// Line that extends with player finger
	Line line;
	Line tempLine;

	// Use this for initialization
	void Start () {
		levelManager = gameObject.GetComponent<LevelManager>();

		clearButton = GameObject.Find("ClearButton").GetComponent<Button>();
		clearButton.onClick.AddListener(ClearButtonClick);

		undoButton = GameObject.Find("UndoButton").GetComponent<Button>();
		undoButton.onClick.AddListener(UndoButtonClick);

		LINE = Resources.Load("Prefabs/Line") as Object;
		TEMP_LINE = Resources.Load("Prefabs/TempLine") as Object;

		lineObject = Instantiate(LINE, Vector3.zero, Quaternion.identity) as GameObject;
		line = lineObject.GetComponent<Line>();
		line.SetLayer(0);

		tempLineObject = Instantiate(TEMP_LINE, Vector3.zero, Quaternion.identity) as GameObject; 
		tempLine = tempLineObject.GetComponent<Line>();
		tempLine.SetLayer(1);
	}
	
	// Update is called once per frame
	void Update () {
		tempLine.ClearLine();
		if (levelManager.GetSelectedNodeObject() != null && levelManager.GetCursor().activeInHierarchy)
		{
			tempLine.AddPoint(levelManager.GetSelectedNodeObject());
			tempLine.AddPoint(levelManager.GetCursor());
		}
	}

	// Add a new point to the line renderer list and update
	public void AddPoint(GameObject point)
	{
		line.AddPoint(point);
	}

	public void RemovePoint(GameObject point)
	{
		line.RemovePoint(point);
		//UpdateLine();
	}

	public void UndoButtonClick()
	{
		RemovePoint(levelManager.GetSelectedNodeObject());

		// Remove point from line renderer
		tempLine.ClearLine();
		tempLine.AddPoint(levelManager.GetSelectedNodeObject());
	}

	// Reset the puzzle
	public void ClearButtonClick()
	{
		line.ClearLine();
		tempLine.ClearLine();
	}

	public void ClearLines()
	{
		line.ClearLine();
		tempLine.ClearLine();
	}
}
