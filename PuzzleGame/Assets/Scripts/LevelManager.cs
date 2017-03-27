// NOTE: 3/17 selectedNode is a SimpleNode componenent reference and needs to be removed so that the selected node
// can support more than just simple nodes. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour 
{

	GameObject gameManagerObject;
	GameManager gameManager;
	PuzzleDatabase puzzleDatabase;

	Object SIMPLE_NODE;
	Object DIAGONAL_NODE;
	Object MOVING_SIMPLE_NODE;
	Object EMPTY_NODE;

	Object SELECT_CIRCLE;
	GameObject selectCircle;	// Circle showing selected node

	Object START_ICON;
	GameObject startIcon;

	Object END_ICON;
	GameObject endIcon;

	Object CURSOR;
	GameObject cursor;

	Object LINE;
	Object TEMP_LINE;
	GameObject lineObject;	// Line as nodes are selected
	GameObject tempLineObject;	// Line that extends with player finger
	Line line;
	Line tempLine;

	Button clearButton;
	Button undoButton;
	
	GameObject selectedNodeObject;

	GameObject previousNodeObject;

	Vector3 mousePosition;
	Vector3 cursorPosition;

	GameObject[,] nodes;
	Vector3[,] nodeLocations;

	LoadPuzzle loadPuzzle;

	int nodeCount;
	int rowNumber;
	int colNumber;
	int startRow;
	int startCol;
	int endRow;
	int endCol;


	void Awake()
	{
		nodeCount = 0;
		rowNumber = 0;
		colNumber = 0;
		startRow = 0;
		startCol = 0;
		endRow = 0;
		endCol = 0;

		nodes = new GameObject[0, 0];
		nodeLocations = new Vector3[0, 0];
		selectedNodeObject = null;
		previousNodeObject = null;
	}

	// Use this for initialization
	void Start () 
	{	
		gameManagerObject = GameObject.Find("GameManager") as GameObject;
		gameManager = gameManagerObject.GetComponent<GameManager>();
		puzzleDatabase = gameManagerObject.GetComponent<PuzzleDatabase>();

		CURSOR = Resources.Load("Prefabs/Cursor") as Object;
		cursor = Instantiate(CURSOR, Vector3.zero, Quaternion.identity) as GameObject;
		cursor.SetActive(false);

		START_ICON = Resources.Load("Prefabs/StartIcon") as Object;
		startIcon = Instantiate(START_ICON, Vector3.zero, Quaternion.identity) as GameObject;

		END_ICON = Resources.Load("Prefabs/EndIcon") as Object;
		endIcon = Instantiate(END_ICON, Vector3.zero, Quaternion.identity) as GameObject;

		SELECT_CIRCLE = Resources.Load("Prefabs/SelectCircle") as Object;
		selectCircle = Instantiate(SELECT_CIRCLE, Vector3.zero, Quaternion.identity) as GameObject;
		selectCircle.SetActive(false);

		LINE = Resources.Load("Prefabs/Line") as Object;
		TEMP_LINE = Resources.Load("Prefabs/TempLine") as Object;

		lineObject = Instantiate(LINE, Vector3.zero, Quaternion.identity) as GameObject;
		line = lineObject.GetComponent<Line>();
		line.SetLayer(0);

		tempLineObject = Instantiate(TEMP_LINE, Vector3.zero, Quaternion.identity) as GameObject; 
		tempLine = tempLineObject.GetComponent<Line>();
		tempLine.SetLayer(1);

		SIMPLE_NODE = Resources.Load("Prefabs/SimpleNode");
		DIAGONAL_NODE = Resources.Load("Prefabs/DiagonalNode");
		MOVING_SIMPLE_NODE = Resources.Load("Prefabs/MovingSimpleNode");
		EMPTY_NODE = Resources.Load("Prefabs/EmptyNode");

		mousePosition = Vector3.zero;
		cursorPosition = Vector3.zero;

		clearButton = GameObject.Find("ClearButton").GetComponent<Button>();
		clearButton.onClick.AddListener(ClearButtonClick);

		undoButton = GameObject.Find("UndoButton").GetComponent<Button>();
		undoButton.onClick.AddListener(UndoButtonClick);
		// Load puzzle
		loadPuzzle = GameObject.Find("LoadPuzzle").GetComponent<LoadPuzzle>();

		string puzzleLocation = puzzleDatabase.GetXml(gameManager.CurrentPuzzle);
		SetNodes(puzzleLocation);
	}
	
	// Update is called once per frame
	void Update () 
	{
		SnapCursor();
		TouchInput();
		GetCursor();
		AdjustCursor();
		SnapCursor();
		BuildPath();
		UpdateTempLine();
		CursorColliderUpdate();
		UpdateSelectCircle();
		CheckWin();

	}

	// Check if all nodes have been crossed the correct amount of times
	void CheckWin()
	{
		int count = 0;
		foreach(GameObject node in nodes)
		{
			
				Node nodeScript = node.GetComponent<Node>();
				if (nodeScript.CurrentCount > 0 && nodeScript.CountType == "NORMAL")
				{
					count++;
				}
			
		}
		if (selectedNodeObject != null)
		{
			Node nodeScript = selectedNodeObject.GetComponent<Node>();
			if (count == 0 && nodeScript.Row == endRow && nodeScript.Column == endCol)
			{
				Debug.Log("You win!");
			}

		}
	}

	void CursorColliderUpdate()
	{
		if (selectedNodeObject == null)
		{
			BoxCollider2D cursorBox = cursor.GetComponent<BoxCollider2D>();
			BoxCollider2D nodeBox;
			Node nodeScript;
			foreach (GameObject node in nodes)
			{
				nodeScript = node.GetComponent<Node>();
				if (nodeScript.LeaveType != "EMPTY")
				{
					nodeBox = node.GetComponent<BoxCollider2D>();
					if (cursorBox.bounds.Intersects(nodeBox.bounds))
					{
						SelectNode(node);
					}
				}
			}
		}
	}

	// Get mouse and convert to cursor at start of frame
	private void GetCursor()
	{
		mousePosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursorPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
	}

	// Cirlce on selected objecct
	private void UpdateSelectCircle()
	{
		if (selectedNodeObject != null)
		{
			if (selectCircle.activeInHierarchy)
				selectCircle.transform.position = selectedNodeObject.transform.position;
		}
	}

	private void UpdateTempLine()
	{
		tempLine.ClearLine();
		if (selectedNodeObject != null && cursor.activeInHierarchy)
		{
			tempLine.AddPoint(selectedNodeObject);
			tempLine.AddPoint(cursor);
		}
	}

	// ********************************************************************
	// CURSOR ADJUSTING
	// ********************************************************************

	// Activate the cursor object
	private void TouchInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			// Dont let the cursor become active unless they select a starting node
			if (selectedNodeObject != null)
			{	
				BoxCollider2D selectedNodeBox = selectedNodeObject.GetComponent<BoxCollider2D>();
				if (selectedNodeBox.bounds.Contains(cursorPosition))
				{
					cursor.SetActive(true);
				}
			}
			else
			{
				cursor.SetActive(true);
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			cursor.SetActive(false);
			if (selectedNodeObject != null)
			{
				cursor.transform.position = selectedNodeObject.transform.position;
			}
		}
	}

	// Have node move relative to mouse
	private void AdjustCursor()
	{
		// If the player is touching screen move the cursor
		if (cursor.activeInHierarchy)
		{
			if (selectedNodeObject == null) // If no node is selected
			{
				cursor.transform.position = cursorPosition;
				return;
			}
			else  	// If a node is selected
			{
				// Do not pick up small error because player's finger is inaccurate
				if (Vector3.Magnitude(cursorPosition - selectedNodeObject.transform.position) < 0.3f)
				{
					cursorPosition = selectedNodeObject.transform.position;
					cursor.transform.position = cursorPosition;
					return;
				}


				Node nodeScript = selectedNodeObject.GetComponent<Node>();
				if (nodeScript.LeaveType == "SIMPLE")
					SimpleCursorAdjust();
				else if (nodeScript.LeaveType == "DIAGONAL")
					DiagonalCursorAdjust();
				
			}
		}
	}

	private void SimpleCursorAdjust()
	{
		Vector3 relativePosition = Vector3.zero;
		// If not moving in a direction yet use new mouse cursor position
		if (cursor.transform.position == selectedNodeObject.transform.position)
		{
				relativePosition = cursorPosition - selectedNodeObject.transform.position;
		}
		// If already moving in a direction use the current cursor position
		else
		{
				relativePosition = cursor.transform.position - selectedNodeObject.transform.position;
		}

		// Store x  and y cursor
		float cursorX = cursorPosition.x;
		float cursorY = cursorPosition.y;

		// If directly on top of the node
		if (cursorX == selectedNodeObject.transform.position.x &&
				cursorY == selectedNodeObject.transform.position.y)
		{
				cursor.transform.position = cursorPosition;
				return;
		}

		float angle = Vector3.Angle(selectedNodeObject.transform.up, relativePosition);

		if ((angle >= 0 && angle <=45.0f) || (angle >= 135.0f && angle <= 180.0f))
		{
				cursorX = selectedNodeObject.transform.position.x;
				cursor.transform.position = new Vector3(cursorX, cursorY, 0);
		}
		else
		{
				cursorY = selectedNodeObject.transform.position.y;
				cursor.transform.position = new Vector3(cursorX, cursorY, 0);
		}
	}

	private void DiagonalCursorAdjust()
	{
		Vector3 relativePosition = Vector3.zero;
		if (cursor.transform.position == selectedNodeObject.transform.position)
		{
			relativePosition = cursorPosition - selectedNodeObject.transform.position;
		}
		else
		{
			relativePosition = cursor.transform.position - selectedNodeObject.transform.position;
		}
				// Store x  and y cursor
				float cursorX = cursorPosition.x;
				float cursorY = cursorPosition.y;

				// If directly on top of the node
				if (cursorX == selectedNodeObject.transform.position.x &&
					cursorY == selectedNodeObject.transform.position.y)
				{
						cursor.transform.position = cursorPosition;
						return;
				}

				// Adjust diagonals based on selected node position
				cursorX -= selectedNodeObject.transform.position.x;
				cursorY -= selectedNodeObject.transform.position.y;

				float angle = Vector3.Angle(selectedNodeObject.transform.up, relativePosition);

				// Top left and right
				if (angle <=90.0f)
				{
						cursorY = Mathf.Abs(cursorX) * Mathf.Tan(Mathf.PI/4);

						// Back to world coordinates
						cursorX += selectedNodeObject.transform.position.x;
						cursorY += selectedNodeObject.transform.position.y;
						 
						cursor.transform.position = new Vector3(cursorX, cursorY, 0);
				}
				// Bottom left and right
				else
				{
						cursorY = Mathf.Abs(cursorX) * Mathf.Tan(Mathf.PI/4);

						// Back to world coordinates
						cursorX += selectedNodeObject.transform.position.x;
						cursorY *= -1;
						cursorY += selectedNodeObject.transform.position.y;

						new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = new Vector3(cursorX, cursorY, 0);
				}
	}

	// Snap cursor to node when within bounds
	private void SnapCursor()
	{
		if (selectedNodeObject != null)
		{
			BoxCollider2D cursorBox = cursor.GetComponent<BoxCollider2D>();
			BoxCollider2D selectedNodeBox = selectedNodeObject.GetComponent<BoxCollider2D>();
			if (cursorBox.bounds.Intersects(selectedNodeBox.bounds))
			{
				cursor.transform.position = selectedNodeObject.transform.position;
			}
		}
	}

	// Recursively selecte nodes that have been crossed over, or block movement
	private void BuildPath()
	{
		if (selectedNodeObject == null || !(cursor.activeInHierarchy))
		{
			return;
		}

		BoxCollider2D cursorBox = cursor.GetComponent<BoxCollider2D>();
		BoxCollider2D selectedNodeBox = selectedNodeObject.GetComponent<BoxCollider2D>();
		if (!cursorBox.bounds.Intersects(selectedNodeBox.bounds))
		{
			Vector2 origin = new Vector2(selectedNodeObject.transform.position.x, selectedNodeObject.transform.position.y);
			Vector2 cursor2D = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
			Vector2 direction = cursor2D - origin;

			RaycastHit2D hit = Physics2D.Raycast(origin, direction, direction.magnitude);

			if (hit != null)
			{
				if (hit.transform.gameObject.tag == "Node")
				{
					//Debug.DrawRay(origin, direction, Color.red, 3);
					// Change cursor location based on this
					SelectNode(hit.transform.gameObject);
					if (hit.transform.gameObject == selectedNodeObject)
					{
						// recursively call self until full path built
						AdjustCursor();
						SnapCursor();
						BuildPath();
					}
					else
					{
						cursor.transform.position = hit.point;
					}
				}
			
			}
		}
		
	}

	// Load and create puzzle
	void SetNodes(string xmlFile)
	{	
		loadPuzzle.ReadItems(xmlFile);
		List<Dictionary<string, string>> masterList = loadPuzzle.GetPuzzleNodes();
		Dictionary<string, string> tempDictionary = new Dictionary<string, string>();

		float distance = 0;

		// Set up the puzzle info
		tempDictionary = loadPuzzle.GetPuzzleInfo();

		rowNumber = int.Parse(tempDictionary["Row"]);
		colNumber = int.Parse(tempDictionary["Column"]);
		startRow = int.Parse(tempDictionary["StartRow"]);
		startCol = int.Parse(tempDictionary["StartColumn"]);
		endRow = int.Parse(tempDictionary["EndRow"]);
		endCol = int.Parse(tempDictionary["EndColumn"]);
		distance = int.Parse(tempDictionary["Distance"]);
		nodeCount = (colNumber) * (rowNumber);
				

		nodeLocations = new Vector3[rowNumber, colNumber];
		nodes = new GameObject[rowNumber, colNumber];

		// Set grid locations
		float moveDirX = 0;
		float moveDirY = 0;

		Vector3 moveDir;

		// Calculate vector to move all locations to center of screen
		if (colNumber%2 ==1)
		{
			moveDirX = ((colNumber-1)/2.0f) * (-distance);
		}
		else
		{
			moveDirX = ((colNumber/2.0f) * (-distance)) + (distance/2.0f);
		}

		if (rowNumber%2 ==1)
		{
			moveDirY = ((rowNumber-1)/2.0f) * (distance);
		}
		else
		{
			moveDirY = ((rowNumber/2.0f) * (distance)) - (distance/2.0f);
		}

		// Direction vector to adjust to center of screen
		moveDir = new Vector3(moveDirX, moveDirY, 0);

		// Set locations
		for (int row = 0; row < rowNumber; row++)
		{
			for (int col = 0; col < colNumber; col++)
			{
				Vector3 tempLocation = new Vector3(col * distance, -row * distance, 0);
				tempLocation += moveDir;
				nodeLocations[row,col] = tempLocation;
			}
		}

		// Create nodes in correct locations
		foreach (Dictionary<string,string> node in masterList)
		{
			if (node["LeaveType"] == "SIMPLE" && node["MoveType"] == "STATIC")
			{
				GameObject tempNode = Instantiate(SIMPLE_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				Node simpleNodeScript = tempNode.GetComponent<Node>();

				simpleNodeScript.SetupNode(int.Parse(node["Count"]), 
										int.Parse(node["Row"]), 
										int.Parse(node["Column"]), 
										node["LeaveType"],
										node["MoveType"],
										node["SpecialType"],
										node["CountType"]);
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
			else if (node["LeaveType"] == "DIAGONAL" && node["MoveType"] == "STATIC")
			{
				GameObject tempNode = Instantiate(DIAGONAL_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				Node simpleNodeScript = tempNode.GetComponent<Node>();

				simpleNodeScript.SetupNode(int.Parse(node["Count"]), 
										int.Parse(node["Row"]), 
										int.Parse(node["Column"]), 
										node["LeaveType"],
										node["MoveType"],
										node["SpecialType"],
										node["CountType"]);
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
			else if (node["LeaveType"] == "SIMPLE" && node["MoveType"] == "MOVING")
			{
				GameObject tempNode = Instantiate(MOVING_SIMPLE_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				Node simpleNodeScript = tempNode.GetComponent<Node>();
				MovingNode movingNodeScript = tempNode.GetComponent<MovingNode>();
				simpleNodeScript.SetupNode(int.Parse(node["Count"]), 
										int.Parse(node["Row"]), 
										int.Parse(node["Column"]), 
										node["LeaveType"],
										node["MoveType"],
										node["SpecialType"],
										node["CountType"]);
				movingNodeScript.SetupNode(float.Parse(node["MoveSpeed"]));
				int wayPointCount = int.Parse(node["WayPointCount"]);

				for (int i = 0; i < wayPointCount; i++)
				{
					string Col = "Col" + ((i+1).ToString());
					string Row = "Row" + ((i+1).ToString());
					int col = int.Parse(node[Col]);
					int row = int.Parse(node[Row]);
					movingNodeScript.AddWayPoint(nodeLocations[row, col]);
				}
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
			else if (node["LeaveType"] == "EMPTY" && node["MoveType"] == "EMPTY")
			{
				GameObject tempNode = Instantiate(EMPTY_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				Node simpleNodeScript = tempNode.GetComponent<Node>();

				simpleNodeScript.SetupNode(int.Parse(node["Count"]), 
										int.Parse(node["Row"]), 
										int.Parse(node["Column"]), 
										node["LeaveType"],
										node["MoveType"],
										node["SpecialType"],
										node["CountType"]);
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
		}

		// Place start and end markers
		startIcon.transform.position = nodeLocations[startRow, startCol];
		endIcon.transform.position = nodeLocations[endRow, endCol];
	}

	// When current puzzle is finished or player exits
	void ClearNodes()
	{
		line.ClearLine();
		tempLine.ClearLine();

		selectedNodeObject = null;
		previousNodeObject = null;

		selectCircle.SetActive(false);

		foreach (GameObject node in nodes)
		{
			if (node != null)
				Destroy(node);
		}
	}

	// Return true to node if able to select new node
	public void SelectNode(GameObject node)
	{
		
		if (node.tag == "Node")
		{
			SelectSimpleNode(node);
		}
		else
		{
			return;
		}
	}

	public void SelectSimpleNode(GameObject node)
	{
		Node newNodeScript = node.GetComponent<Node>();
		if (newNodeScript.CountType == "NORMAL" && newNodeScript.CurrentCount <=0 )
		{
				return;
		}

		// First node of game
		if (selectedNodeObject == null)
		{
			previousNodeObject = selectedNodeObject;
			selectedNodeObject = node;

			if (newNodeScript.Row == startRow && newNodeScript.Column == startCol)
			{
				// Let node know it has been selected
				newNodeScript.DecreaseCount();

				// Adjust cursor position
				cursor.transform.position = node.transform.position;

				// Add new point to line renderer
				line.AddPoint(selectedNodeObject);
				//Debug.Log(selectedNode.GridLocation.ToString());

				// Set the select cirlce
				selectCircle.SetActive(true);
				selectCircle.transform.position = selectedNodeObject.transform.position;

				//tempLine.AddPoint(selectedNodeObject);
				return;
			}
			else
			{
				previousNodeObject = null;
				selectedNodeObject = null;
				return;
			}
		}
		else
		{
			Node selectedNode = selectedNodeObject.GetComponent<Node>();

			// Node that is already selected
			if (node == selectedNodeObject)
			{
				return;
			}

			// Set previous node
			previousNodeObject = selectedNodeObject;	
			// Set new node
			selectedNodeObject = node;

			// Let node know it has been selected
			//Debug.Log(newNodeScript.Row + ", " + newNodeScript.Column);
			newNodeScript.DecreaseCount();

			// Move the cursor to the new node
			cursor.transform.position = selectedNode.transform.position;

			// Add new point to line renderer
			line.AddPoint(selectedNodeObject);
			//tempLine.ClearLine();
			//tempLine.AddPoint(selectedNodeObject);

			return;
		}
	}

	public void UndoButtonClick()
	{
		if (previousNodeObject != null)
		{
					Node selectedNode = selectedNodeObject.GetComponent<Node>();

					// Set new node
					selectedNode.Undo();
					line.RemovePoint(selectedNodeObject);

					selectedNodeObject = previousNodeObject;
		
					cursor.transform.position = selectedNode.transform.position;

					// Remove point from line renderer
					tempLine.ClearLine();
					tempLine.AddPoint(selectedNodeObject);

					// Update select cirlce
					selectCircle.transform.position = selectedNodeObject.transform.position;

					previousNodeObject = null;
				
		}
	}

	// Reset the puzzle
	public void ClearButtonClick()
	{
		foreach(GameObject node in nodes)
		{
			
			Node nodeScript = node.GetComponent<Node>();
			nodeScript.Reset();
			
		}
		selectedNodeObject = null;
		previousNodeObject = null;

		selectCircle.SetActive(false);

		line.ClearLine();
		tempLine.ClearLine();
	}
}
