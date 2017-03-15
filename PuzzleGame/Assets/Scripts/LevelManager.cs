// NOTE: 3/15 Need to add check for diagonal in select node
// Need to add snapping to diagonal already moving

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour 
{
	Object SIMPLE_NODE;

	Object CURSOR;
	GameObject cursor;
	Object LINE;
	GameObject lineObject;	// Line as nodes are selected
	GameObject tempLineObject;	// Line that extends with player finger
	Line line;
	Line tempLine;

	Button clearButton;
	
	GameObject selectedNodeObject;
	SimpleNode selectedNode;

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
		selectedNode = null;
	}

	// Use this for initialization
	void Start () 
	{	
		CURSOR = Resources.Load("Prefabs/Cursor") as Object;
		cursor = Instantiate(CURSOR, Vector3.zero, Quaternion.identity) as GameObject;
		cursor.SetActive(false);

		LINE = Resources.Load("Prefabs/Line") as Object;
		lineObject = Instantiate(LINE, Vector3.zero, Quaternion.identity) as GameObject;
		line = lineObject.GetComponent<Line>();

		tempLineObject = Instantiate(LINE, Vector3.zero, Quaternion.identity) as GameObject; 
		tempLine = tempLineObject.GetComponent<Line>();

		SIMPLE_NODE = Resources.Load("Prefabs/SimpleNode");

		// Load puzzle
		loadPuzzle = GameObject.Find("LoadPuzzle").GetComponent<LoadPuzzle>();
		SetNodes();

		mousePosition = Vector3.zero;
		cursorPosition = Vector3.zero;

		clearButton = GameObject.Find("ClearButton").GetComponent<Button>();
		clearButton.onClick.AddListener(ClearButtonClick);
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
			SimpleNode nodeScript = node.GetComponent<SimpleNode>();
			if (nodeScript.CurrentCount > 0)
			{
				count++;
			}
		}
		if (selectedNodeObject != null)
		{
			if (count == 0 && selectedNode.Row == endRow && selectedNode.Column == endCol)
			{
				Debug.Log("You win!");
			}
		}
	}

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
			if (selectedNode != null)
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
			if (selectedNode == null) // If no node is selected
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

				if (selectedNode.Type == "SIMPLE")
					SimpleCursorAdjust();
				else if (selectedNode.Type == "DIAGONAL")
					DiagonalCursorAdjust();
			}
		}
	}

	private void SimpleCursorAdjust()
	{
		if (cursor.transform.position == selectedNodeObject.transform.position)
		{
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

				Vector3 relativePosition = cursorPosition - selectedNodeObject.transform.position;

				float angle = Vector3.Angle(selectedNodeObject.transform.up, relativePosition);

				if ((angle >= 0 && angle <=45.0f) || (angle >= 135.0f && angle <= 180.0f))
				{
					//Debug.Log("UP DOWN " + angle.ToString());
					cursorX = selectedNodeObject.transform.position.x;
					cursorPosition = new Vector3(cursorX, cursorY, 0);
				}
				else
				{
					//Debug.Log("LEFT RIGHT "+angle.ToString());
					cursorY = selectedNodeObject.transform.position.y;
					cursorPosition = new Vector3(cursorX, cursorY, 0);
				}

				cursor.transform.position = cursorPosition;
		}
		// Already  moving in a specific direction
		else
		{
				// Already moving left/right
				if (cursor.transform.position.x == selectedNodeObject.transform.position.x)
				{
						// Get mouse position and convert to world
						float cursorX = cursorPosition.x;
						float cursorY = cursorPosition.y;

						cursorX = selectedNodeObject.transform.position.x;
						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;
				}
				// Already moving up/down
				else
				{
					// Get mouse position and convert to world
					float cursorX = cursorPosition.x;
					float cursorY = cursorPosition.y;

					cursorY = selectedNodeObject.transform.position.y;
					cursorPosition = new Vector3(cursorX, cursorY, 0);
					cursor.transform.position = cursorPosition;
				}
		}
	}

	private void DiagonalCursorAdjust()
	{
		if (cursor.transform.position == selectedNodeObject.transform.position)
		{
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

				Vector3 relativePosition = cursorPosition - selectedNodeObject.transform.position;

				float angle = Vector3.Angle(selectedNodeObject.transform.up, relativePosition);

				// Top left
				if (angle >= 0 && angle <=90.0f)
				{
						cursorY = Mathf.Abs(cursorX) / Mathf.Tan(Mathf.Deg2Rad * 45.0f);
						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;
				}
				// Up or down
				else
				{
						cursorY = Mathf.Abs(cursorX) / Mathf.Tan(Mathf.Deg2Rad * 45.0f);
						cursorPosition = new Vector3(cursorX, -cursorY, 0);
						cursor.transform.position = cursorPosition;
				}
		}
		// Already  moving in a specific direction
		else
		{
				// Already moving topleft topright
				if (cursor.transform.position.x == selectedNodeObject.transform.position.x)
				{
						// Get mouse position and convert to world
						float cursorX = cursorPosition.x;
						float cursorY = cursorPosition.y;

						cursorX = selectedNodeObject.transform.position.x;
						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;
				}
				// Already moving bottoleft bottomright
				else
				{
						// Get mouse position and convert to world
						float cursorX = cursorPosition.x;
						float cursorY = cursorPosition.y;

						cursorY = selectedNodeObject.transform.position.y;
						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;
				}
		}
	}

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
					bool successful = SelectNode(hit.transform.gameObject);
					if (successful)
					{
						node.DecreaseCount();

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
		loadPuzzle.ReadItems("XML/TestLevel1");
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
		float distance = 3;
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
			if (node["Type"] == "SIMPLE" || node["Type"] == "DIAGONAL")
			{
				GameObject tempNode = Instantiate(SIMPLE_NODE, nodeLocations[int.Parse(node["Row"]), int.Parse(node["Column"])], Quaternion.identity) as GameObject;
				SimpleNode simpleNodeScript = tempNode.GetComponent<SimpleNode>();

				simpleNodeScript.SetupNode(int.Parse(node["Count"]), int.Parse(node["Row"]), int.Parse(node["Column"]), node["Type"]);
				nodes[int.Parse(node["Row"]), int.Parse(node["Column"])] = tempNode;
			}
		}

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
	public bool SelectNode(GameObject node)
	{
		SimpleNode otherNode = node.GetComponent<SimpleNode>();

		//check for simple node
		if (otherNode == null)
				return false;
		if (otherNode.CurrentCount <=0 )
		{
				return false;
		}

		// First node of game
		if (selectedNodeObject == null)
		{
			selectedNodeObject = node;
			selectedNode = selectedNodeObject.GetComponent<SimpleNode>();

			if (selectedNode.Row == startRow && selectedNode.Column == startCol)
			{
				// Adjust cursor position
				cursor.transform.position = node.transform.position;

				// Add new point to line renderer
				line.AddPoint(selectedNodeObject.transform.position);
				//Debug.Log(selectedNode.GridLocation.ToString());

				tempLine.AddPoint(selectedNodeObject.transform.position);
				return true;
			}
			else
			{
				selectedNodeObject = null;
				selectedNode = null;
				return false;
			}
		}
		else
		{
			// Left and right movement
			if (selectedNode.Type == "SIMPLE")
			{
				// Node that is already selected
				if (otherNode.Row == selectedNode.Row && otherNode.Column == selectedNode.Column)
				{
					cursor.transform.position = selectedNode.transform.position;
					return false;
				}

				// UP DOWN
				if (otherNode.Column == selectedNode.Column &&
					(otherNode.Row == (selectedNode.Row - 1) || otherNode.Row == selectedNode.Row + 1))
					{
						
						selectedNodeObject = node;
						selectedNode = node.GetComponent<SimpleNode>();
						cursor.transform.position = selectedNode.transform.position;

						// Add new point to line renderer
						line.AddPoint(selectedNodeObject.transform.position);
						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);

						return true;
					}
				// LEFT RIGHT
				if (otherNode.Row == selectedNode.Row && (otherNode.Column == selectedNode.Column -1) ||
					otherNode.Column == (selectedNode.Column + 1))
				{
						selectedNodeObject = node;
						selectedNode = node.GetComponent<SimpleNode>();
						cursor.transform.position = selectedNode.transform.position;

						// Add new point to line renderer
						line.AddPoint(selectedNodeObject.transform.position);
						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);

						return true;
					}
			}
		}
		return false;
	}

	// Reset the puzzle
	public void ClearButtonClick()
	{
		foreach(GameObject node in nodes)
		{
			SimpleNode nodeScript = node.GetComponent<SimpleNode>();
			nodeScript.ResetCount();
		}
		selectedNode = null;
		selectedNodeObject = null;

		line.ClearLine();
		tempLine.ClearLine();
	}
}
