using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
	LoadPuzzleManager _loadPuzzleManager; 

	Object BASE_CIRCLE;

	float tempX, tempY;

	private Color[] colorList;

	// Node Information
	GameObject[,] nodes; 
	Vector3[,] nodeLocations; 
	int _rowNum, _colNum, _nodeCount, _startRow, _startCol, _endRow, _endCol; 

	// Line Render
	private LineRenderer lr;
	private int lineCounter = 0; 

	// User's Input
	private bool isSwiping, isGameOVer = false;
	GameObject previousNodeObject, selectedNodeObject;
	RaycastHit2D hit;

	void Awake ()
	{
		// Map's Information
		_nodeCount = 0;
		_rowNum = 0;
		_colNum = 0;
		_startRow = 0;
		_startCol = 0;
		_endRow = 0;
		_endCol = 0;

		nodes = new GameObject[0, 0];
		nodeLocations = new Vector3[0,0];

		previousNodeObject = null; 
		selectedNodeObject = null;

		// Load Puzzle Manager
		_loadPuzzleManager = GameObject.Find("TestManager").GetComponent<LoadPuzzleManager>();

		// Line Render
		lr = GameObject.Find("LineDisplay").GetComponent<LineRenderer>();
		if (lr == null)
			Debug.Log ("ERROR - CANNOT FIND LINE DISPLAY GAMEOBJECT IN SCENE");

		colorList = new Color[]{ Color.black, Color.red, Color.blue };

	}

	// Use this for initialization
	void Start ()
	{
		BASE_CIRCLE = Resources.Load ("Prefabs/Circle");
		SetCircle ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckMouseSwipe ();
	}

	// *********************************************************************************************
	// SetCircle() Function - Create the puzzle layout
	// *********************************************************************************************
	void SetCircle ()
	{
		// Load Puzzle from xml file
		_loadPuzzleManager.LoadPuzzle ("XML/PuzzleEasy1");
		List<Dictionary<string,string>> _masterList = _loadPuzzleManager.GetPuzzleNodes ();
		Dictionary<string, string> _tempDictionary = new Dictionary<string, string> ();

		// Find the Map Node
		foreach (Dictionary<string,string> _node in _masterList) {
			if (_node ["type"] == "MAP") {
				_tempDictionary = _node;
				_rowNum = int.Parse (_tempDictionary ["row"]);
				_colNum = int.Parse (_tempDictionary ["col"]);
				break;
			}
		}

		// Find the Start Node
		foreach (Dictionary<string,string> _node in _masterList) {
			if (_node ["type"] == "START") {
				_tempDictionary = _node;
				_startRow = int.Parse (_tempDictionary ["row"]);
				_startCol = int.Parse (_tempDictionary ["col"]);
				break;
			}
		}

		// Find the End Node
		foreach (Dictionary<string,string> _node in _masterList) {
			if (_node ["type"] == "END") {
				_tempDictionary = _node;
				_endRow = int.Parse (_tempDictionary ["row"]);
				_endCol = int.Parse (_tempDictionary ["col"]);
				break;
			}
		}

		// Set Node's Stuff
		_nodeCount = _rowNum * _colNum;
		nodeLocations = new Vector3[_rowNum, _colNum];
		nodes = new GameObject[_rowNum, _colNum];

		// Set grid locations
		float distance = 2;
		float moveDirX = 0;
		float moveDirY = 0;

		Vector3 moveDir; 


		// Calculate vector to move all locations to center of screen
		if (_rowNum % 2 == 1)
			moveDirX = ((_colNum - 1) / 2.0f) * (-distance);
		else
			moveDirX = ((_colNum / 2.0f) * (-distance)) - (distance / 2.0f);

		if (_colNum % 2 == 1)
			moveDirY = ((_rowNum - 1) / 2.0f) * (distance);
		else
			moveDirY = ((_rowNum / 2.0f) * (distance)) - (distance / 2.0f);

		moveDir = new Vector3 (moveDirX, moveDirY, 0);

		// Set locations
		for (int row = 0; row < _rowNum; row++) {
			for (int col = 0; col < _colNum; col++) {
				Vector3 tempLocation = new Vector3 (col * distance, -row * distance, 0);
				tempLocation += moveDir;
				nodeLocations [row, col] = tempLocation; 
			}
		}
			
		// Create node in correct location
		foreach (Dictionary<string, string> node in _masterList) {
			if (node ["type"] == "SIMPLE") {
				GameObject tempNode = Instantiate (BASE_CIRCLE, nodeLocations [int.Parse (node ["row"]), int.Parse (node ["col"])], Quaternion.identity) as GameObject;  
				CircleClass ccScript = tempNode.GetComponent<CircleClass> ();

				ccScript.SetUpCircle (int.Parse (node ["count"]), int.Parse (node ["row"]), int.Parse (node ["col"]));
				tempNode.GetComponent<SpriteRenderer> ().color = colorList [int.Parse (node ["count"])];

				nodes [int.Parse (node ["row"]), int.Parse (node ["col"])] = tempNode; 
			} else if (node ["type"] == "DIAGONAL") {
				GameObject tempNode = Instantiate (BASE_CIRCLE, nodeLocations [int.Parse (node ["row"]), int.Parse (node ["col"])], Quaternion.identity) as GameObject;  
				CircleClass ccScript = tempNode.GetComponent<CircleClass> ();

				ccScript.SetUpCircle (int.Parse (node ["count"]), int.Parse (node ["row"]), int.Parse (node ["col"]));
				nodes [int.Parse (node ["row"]), int.Parse (node ["col"])] = tempNode; 
			}
		}
	}


	// *********************************************************************************************
	// HitCircle() Function - When player hit a circle, calculate circle's hit count & display line 
	// *********************************************************************************************
	void HitCircle (GameObject gameObj)
	{
		CircleClass currentHitScript = gameObj.GetComponent<CircleClass> ();
		currentHitScript.HitCount--;
		gameObj.GetComponent<SpriteRenderer> ().color = colorList [currentHitScript.HitCount];

		// Display Line
		int index = lineCounter;
		lineCounter++;
		lr.numPositions = lineCounter;
		lr.SetPosition (index, gameObj.transform.localPosition);
	}
		

	// *********************************************************************************************
	// CheckMouseSwipe() Function - Check for user mouse's input
	// *********************************************************************************************
	void CheckMouseSwipe ()
	{
		if (Input.GetMouseButtonDown (0)) {
			//startPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			isSwiping = true; 
		}

		if (isSwiping) {
			//endPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			if (!isGameOVer) {
				CheckObjClick ();
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			isSwiping = false;
			previousNodeObject = null;
			selectedNodeObject = null;
			ResetLevel ();
		}
	}


	// *********************************************************************************************
	// CheckObjClick() Function - Check for object being clicked
	// *********************************************************************************************
	void CheckObjClick ()
	{
		hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.down);
		if (hit.collider != null) {

			CircleClass tempCC = hit.collider.gameObject.GetComponent<CircleClass> ();

			// When start object is null, calculate start object
			if (previousNodeObject == null) {
				if (tempCC.GridRow == _startRow && tempCC.GridCol == _startCol) {
					previousNodeObject = hit.collider.gameObject;
					HitCircle (previousNodeObject);
				} else
					previousNodeObject = null;
			} 

			// When end object is null, calculate end object
			else if (previousNodeObject != null && selectedNodeObject == null && previousNodeObject != hit.collider.gameObject) {
				if (tempCC.GridRow != _endRow || tempCC.GridCol != _endCol) {
					if (tempCC.HitCount > 0 && CheckCircleAdjacent (previousNodeObject, hit.collider.gameObject)) {
						selectedNodeObject = hit.collider.gameObject;
						HitCircle (selectedNodeObject);							
					} else
						selectedNodeObject = previousNodeObject;
				} else {
					selectedNodeObject = hit.collider.gameObject;
					HitCircle (selectedNodeObject);
					CalculateWin ();
					isGameOVer = true;
					Invoke ("ResetLevel", 1.5f);

				}
			} 

			// When start object and end object are not null, do this
			else if (previousNodeObject != null && selectedNodeObject != null && selectedNodeObject != hit.collider.gameObject) {
				if (tempCC.GridRow != _endRow || tempCC.GridCol != _endCol) {
					previousNodeObject = selectedNodeObject;

					if (tempCC.HitCount > 0 && CheckCircleAdjacent (selectedNodeObject, hit.collider.gameObject)) {
						//previousNodeObject = selectedNodeObject;
						selectedNodeObject = hit.collider.gameObject;
						HitCircle (selectedNodeObject);
					} else
						selectedNodeObject = previousNodeObject;
				} else {
					if (CheckCircleAdjacent (selectedNodeObject, hit.collider.gameObject)) {
						selectedNodeObject = hit.collider.gameObject;
						HitCircle (selectedNodeObject);
						CalculateWin ();
						isGameOVer = true;
						Invoke ("ResetLevel", 1.5f);
					}
				}

			}
		}
	}

	// *********************************************************************************************
	// CheckCircleAdjacent(GameObject, GameObject) Function - Check if obj2 is adjacent to obj1
	// *********************************************************************************************
	bool CheckCircleAdjacent(GameObject obj1, GameObject obj2){
		CircleClass obj1cc = obj1.GetComponent<CircleClass> ();
		CircleClass obj2cc = obj2.GetComponent<CircleClass> ();


		if (((obj2cc.GridRow == obj1cc.GridRow) && ((obj2cc.GridCol == obj1cc.GridCol - 1) || (obj2cc.GridCol == obj1cc.GridCol + 1))) ||
			(obj2cc.GridCol == obj1cc.GridCol) && ((obj2cc.GridRow == obj1cc.GridRow - 1) || (obj2cc.GridRow == obj1cc.GridRow + 1))){  

			return true;
		}
		else{ 
			return false;
		}
	}

	// *********************************************************************************************
	// CalculateWin() Function - Check if Player's Win
	// *********************************************************************************************
	void CalculateWin ()
	{
		int damageCount = 0;

		foreach (GameObject circle in nodes) {
			CircleClass tempCCScript = circle.GetComponent<CircleClass> ();
			if (tempCCScript.HitCount == 0) {
				damageCount++;
			}
		}

		Debug.Log ("You dealt " + damageCount + " damage points");

	}

	// *********************************************************************************************
	// ResetLevel() Function - Reset all Node's Information & Line's Information
	// *********************************************************************************************
	void ResetLevel ()
	{
		foreach (GameObject circle in nodes) {
			CircleClass tempCC = circle.GetComponent<CircleClass> ();
			tempCC.Reset ();

			circle.GetComponent<SpriteRenderer> ().color = colorList [tempCC.HitCount];
		}

		// Reset Line Renderer
		lineCounter = 0;
		lr.numPositions = lineCounter;

	}
}




