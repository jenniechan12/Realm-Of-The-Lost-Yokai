// NOTE: 3/17 selectedNode is a SimpleNode componenent reference and needs to be removed so that the selected node
// can support more than just simple nodes. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour 
{
	Object SIMPLE_NODE;
	Object DIAGONAL_NODE;

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

		mousePosition = Vector3.zero;
		cursorPosition = Vector3.zero;

		clearButton = GameObject.Find("ClearButton").GetComponent<Button>();
		clearButton.onClick.AddListener(ClearButtonClick);

		undoButton = GameObject.Find("UndoButton").GetComponent<Button>();
		undoButton.onClick.AddListener(UndoButtonClick);
		// Load puzzle
		loadPuzzle = GameObject.Find("LoadPuzzle").GetComponent<LoadPuzzle>();
		SetNodes();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GetCursor();
		TouchInput();
		AdjustCursor();
		SnapCursor();
		BuildPath();
		UpdateTempLine();
		CheckWin();

	}

	// Check if all nodes have been crossed the correct amount of times
	void CheckWin()
	{
		int count = 0;
		foreach(GameObject node in nodes)
		{
			if (node.tag == "SimpleNode")
			{
				SimpleNode nodeScript = node.GetComponent<SimpleNode>();
				if (nodeScript.CurrentCount > 0)
				{
					count++;
				}
			}
		}
		if (selectedNodeObject != null)
		{
			if (selectedNodeObject.tag == "SimpleNode")
			{
				SimpleNode nodeScript = selectedNodeObject.GetComponent<SimpleNode>();
				if (count == 0 && nodeScript.Row == endRow && nodeScript.Column == endCol)
				{
					Debug.Log("You win!");
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

	private void UpdateTempLine()
	{
		tempLine.ClearLine();
		if (selectedNodeObject != null && cursor.activeInHierarchy)
		{
			tempLine.AddPoint(selectedNodeObject.transform.position);
			tempLine.AddPoint(cursorPosition);
		}
	}

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

				if (selectedNodeObject.tag == "SimpleNode")
				{
					SimpleNode nodeScript = selectedNodeObject.GetComponent<SimpleNode>();
					if (nodeScript.Type == "SIMPLE")
						SimpleCursorAdjust();
					else if (nodeScript.Type == "DIAGONAL")
						DiagonalCursorAdjust();
				}
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
			//Debug.Log("UP DOWN " + angle.ToString());
			cursorX = selectedNodeObject.transform.position.x;
			cursorPosition = new Vector3(cursorX, cursorY, 0);
			cursor.transform.position = cursorPosition;
		}
		else
		{
			//Debug.Log("LEFT RIGHT "+angle.ToString());
			cursorY = selectedNodeObject.transform.position.y;
			cursorPosition = new Vector3(cursorX, cursorY, 0);
			cursor.transform.position = cursorPosition;
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

						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;
				}
				// Bottom left and right
				else
				{
						cursorY = Mathf.Abs(cursorX) * Mathf.Tan(Mathf.PI/4);

						// Back to world coordinates
						cursorX += selectedNodeObject.transform.position.x;
						cursorY *= -1;
						cursorY += selectedNodeObject.transform.position.y;

						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;
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
				if (hit.transform.gameObject.tag == "SimpleNode")
				{
					// Change cursor location based on this
					SimpleNode node = hit.transform.gameObject.GetComponent<SimpleNode>();
					SelectNode(hit.transform.gameObject);
					if (hit.transform.gameObject == selectedNodeObject)
					{
						// recursively call self until full path built
						AdjustCursor();
						BuildPath();
					}
					else
					{
						cursorPosition = hit.point;
						cursor.transform.position = hit.point;
					}
				}
			
			}
		}
		
	}

	// Load and create puzzle
	void SetNodes()
	{	
		loadPuzzle.ReadItems("XML/TestLevel2");
		List<Dictionary<string, string>> masterList = loadPuzzle.GetPuzzleNodes();
		Dictionary<string, string> tempDictionary = new Dictionary<string, string>();

		// Find master node
		foreach (Dictionary<string,string> node in masterList)
		{
			if (node["Type"] == "MASTER")
			{
				tempDictionary = node;
				rowNumber = int.Parse(tempDictionary["Row"]);
				colNumber = int.Parse(tempDictionary["Column"]);
				break;
			}
		}

		// Set the start node
		foreach (Dictionary<string,string> node in masterList)
		{
			if (node["Type"] == "START")
			{
				tempDictionary = node;
				startRow = int.Parse(tempDictionary["Row"]);
				startCol = int.Parse(tempDictionary["Column"]);
				break;
			}
		}

		// Set the last node
		foreach (Dictionary<string,string> node in masterList)
		{
			if (node["Type"] == "END")
			{
				tempDictionary = node;
				endRow = int.Parse(tempDictionary["Row"]);
				endCol = int.Parse(tempDictionary["Column"]);
				nodeCount = (colNumber) * (rowNumber);
				break;
			}
		}

		nodeLocations = new Vector3[rowNumber, colNumber];
		nodes = new GameObject[rowNumber, colNumber];

		// Set grid locations
		float distance = 4;
		float moveDirX = 0;
		float moveDirY = 0;

		Vector3 moveDir;

		// Calculate vector to move all lcoations to center of screen
		if (rowNumber%2 ==1)
		{
			moveDirX = ((colNumber-1)/2.0f) * (-distance);
		}
		else
		{
			moveDirX = ((colNumber/2.0f) * (-distance)) - (distance/2.0f);
		}

		if (colNumber%2 ==1)
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
			if (node["Type"] == "SIMPLE")
			{
				GameObject tempNode = Instantiate(SIMPLE_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				SimpleNode simpleNodeScript = tempNode.GetComponent<SimpleNode>();

				simpleNodeScript.SetupNode(int.Parse(node["Count"]), int.Parse(node["Row"]), int.Parse(node["Column"]), node["Type"]);
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
			else if (node["Type"] == "DIAGONAL")
			{
				GameObject tempNode = Instantiate(DIAGONAL_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				SimpleNode simpleNodeScript = tempNode.GetComponent<SimpleNode>();

				simpleNodeScript.SetupNode(int.Parse(node["Count"]), int.Parse(node["Row"]), int.Parse(node["Column"]), node["Type"]);
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
		}

		// Place start and end markers
		startIcon.transform.position = nodeLocations[startRow, startCol];
		endIcon.transform.position = nodeLocations[endRow, endCol];
	}

	// Called by nodes to keep cursor in node
	public void SetCursor(GameObject node)
	{
		if (node == selectedNodeObject)
		{
				cursor.transform.position = node.transform.position;
		}
	}

	// Return true to node if able to select new node
	public void SelectNode(GameObject node)
	{
		
		if (node.tag == "SimpleNode")
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
		SimpleNode newNodeScript = node.GetComponent<SimpleNode>();
		if (newNodeScript.CurrentCount <=0 )
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
				line.AddPoint(selectedNodeObject.transform.position);
				//Debug.Log(selectedNode.GridLocation.ToString());

				// Set the select cirlce
				selectCircle.SetActive(true);
				selectCircle.transform.position = selectedNodeObject.transform.position;

				tempLine.AddPoint(selectedNodeObject.transform.position);
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
			SimpleNode selectedNode = selectedNodeObject.GetComponent<SimpleNode>();

			// Node that is already selected
			if (node == selectedNodeObject)
			{
				cursor.transform.position = selectedNodeObject.transform.position;
				return;
			}

			// Left and right movement
			if (selectedNode.Type == "SIMPLE")
			{
				// If a neighbor
				if ((newNodeScript.Column == selectedNode.Column &&
					(newNodeScript.Row == (selectedNode.Row - 1) || newNodeScript.Row == selectedNode.Row + 1))
					|| (newNodeScript.Row == selectedNode.Row && (newNodeScript.Column == selectedNode.Column -1) ||
						newNodeScript.Column == (selectedNode.Column + 1)))
					{
						// Set previous node
						previousNodeObject = selectedNodeObject;	
						// Set new node
						selectedNodeObject = node;

						// Let node know it has been selected
						newNodeScript.DecreaseCount();

						// Move the cursor to the new node
						cursor.transform.position = selectedNode.transform.position;

						// Add new point to line renderer
						line.AddPoint(selectedNodeObject.transform.position);
						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);

						// Update select cirlce
						selectCircle.transform.position = selectedNodeObject.transform.position;
						return;
					}
			}
			else if (selectedNode.Type == "DIAGONAL")
			{
				// If a neighbor allow to be selected
				if ((newNodeScript.Column == selectedNode.Column-1 && 
					(newNodeScript.Row == selectedNode.Row -1 
						|| newNodeScript.Row == selectedNode.Row + 1))
					|| (newNodeScript.Column == selectedNode.Column + 1 && 
						(newNodeScript.Row == selectedNode.Row -1 
							|| newNodeScript.Row == selectedNode.Row + 1)))
				{
					// Set previous node
					previousNodeObject = selectedNodeObject;	
					// Set new node	
					selectedNodeObject = node;

					// Let node know it has been selected
					newNodeScript.DecreaseCount();

						// Move the cursor to the new node
					cursor.transform.position = selectedNode.transform.position;

					// Add new point to line renderer
					line.AddPoint(selectedNodeObject.transform.position);
					tempLine.ClearLine();
					tempLine.AddPoint(selectedNodeObject.transform.position);

					// Update select cirlce
					selectCircle.transform.position = selectedNodeObject.transform.position;
					return;
				}
			}
		}
		return;
	}

	public void UndoButtonClick()
	{
		if (previousNodeObject != null)
		{
				if (selectedNodeObject.tag == "SimpleNode")
				{
					SimpleNode selectedNode = selectedNodeObject.GetComponent<SimpleNode>();

					// Set new node
					selectedNode.Undo();
					line.RemovePoint(selectedNodeObject.transform.position);

					selectedNodeObject = previousNodeObject;
		
					cursor.transform.position = selectedNode.transform.position;

					// Remove point from line renderer
					tempLine.ClearLine();
					tempLine.AddPoint(selectedNodeObject.transform.position);

					// Update select cirlce
					selectCircle.transform.position = selectedNodeObject.transform.position;

					previousNodeObject = null;
				}
		}
	}

	// Reset the puzzle
	public void ClearButtonClick()
	{
		foreach(GameObject node in nodes)
		{
			SimpleNode nodeScript = node.GetComponent<SimpleNode>();
			nodeScript.Reset();
		}
		selectedNodeObject = null;

		selectCircle.SetActive(false);

		line.ClearLine();
		tempLine.ClearLine();
	}
}
