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
	GameObject[] nodes;
	Vector3[] nodeLocations;
	int nodeCount;
	int height;
	int width;
	int startNode;
	int endNode;

	void Awake()
	{
		height = 3;
		width = 3;
		nodeCount = height * width;
		nodes = new GameObject[nodeCount];
		nodeLocations = new Vector3[nodeCount];
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
		SetNodes();

		clearButton = GameObject.Find("ClearButton").GetComponent<Button>();
		clearButton.onClick.AddListener(ClearButtonClick);
	}
	
	// Update is called once per frame
	void Update () 
	{
		TouchInput();
		AdjustCursor();
		CheckWin();

	}

	void CheckWin()
	{
		int count = 0;
		foreach(GameObject node in nodes)
		{
			SimpleNode nodeScript = node.GetComponent<SimpleNode>();
			if (nodeScript.CurrentCount > 0)
				count++;
		}
		if (count == 0 && selectedNode.GridLocation == endNode)
			Debug.Log("You win!");
	}

	private void TouchInput()
	{
		if (Input.GetMouseButtonDown(0))
			cursor.SetActive(true);
		if (Input.GetMouseButtonUp(0))
		{
			cursor.SetActive(false);
			if (selectedNodeObject != null)
				cursor.transform.position = selectedNodeObject.transform.position;
		}
	}

	// Have mouse move relative to node
	private void AdjustCursor()
	{
		// If the player is touching screen move the cursor
		if (cursor.activeInHierarchy)
		{
			// If no node is selected
			if (selectedNode == null)
			{
				Vector3 mousePosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 cursorPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
				cursor.transform.position = cursorPosition;
				return;
			}
			else
			{
				// Have some error built in since player finger is not perfect
				Vector3 mPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 cPosition = new Vector3(mPosition.x, mPosition.y, 0);
				if (Vector3.Magnitude(cPosition - selectedNodeObject.transform.position) < 0.5f)
				{
					return;
				}

				// If not moving in a specific direction
				if (cursor.transform.position == selectedNodeObject.transform.position)
				{
					// Get mouse position and convert to world position
					Vector3 mousePosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
					Vector3 cursorPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
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

					tempLine.ClearLine();
					tempLine.AddPoint(selectedNodeObject.transform.position);
					tempLine.AddPoint(cursorPosition);

					cursor.transform.position = cursorPosition;
				}
				// Already  moving in a specific direction
				else
				{
					// Already moving left/right
					if (cursor.transform.position.x == selectedNodeObject.transform.position.x)
					{
						// Get mouse position and convert to world
						Vector3 mousePosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
						Vector3 cursorPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
						float cursorX = cursorPosition.x;
						float cursorY = cursorPosition.y;

						cursorX = selectedNodeObject.transform.position.x;
						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;

						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);
						tempLine.AddPoint(cursorPosition);
					}
					// Already moving up/down
					else
					{
						// Get mouse position and convert to world
						Vector3 mousePosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
						Vector3 cursorPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
						float cursorX = cursorPosition.x;
						float cursorY = cursorPosition.y;

						cursorY = selectedNodeObject.transform.position.y;
						cursorPosition = new Vector3(cursorX, cursorY, 0);
						cursor.transform.position = cursorPosition;

						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);
						tempLine.AddPoint(cursorPosition);
					}

				}
			}

		}
	}

	void SetNodes()
	{	
		// Set starting node
		startNode = 0;
		endNode = 8;

		// Set grid locations
		int i = 0;
		Vector3 tempLocation = new Vector3(-3, -3, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(0, -3, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(3, -3, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(-3, 0, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(0, 0, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(3, 0, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(-3, 3, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(0, 3, 0);
		nodeLocations[i] = tempLocation;
		i++;

		tempLocation = new Vector3(3, 3, 0);
		nodeLocations[i] = tempLocation;
		i++;

		// Create node at each location
		for (int k = 0; k < nodeCount; k++)
		{
			GameObject tempNode;
			tempNode = Instantiate(SIMPLE_NODE, nodeLocations[k], Quaternion.identity) as GameObject;
			SimpleNode simpleNodeScript = tempNode.GetComponent<SimpleNode>();

			simpleNodeScript.SetupNode(1, k);
			nodes[k] = tempNode;
		}
	}

	public void SetCursor(Vector3 location)
	{
		cursor.transform.position = location;
	}

	// Return true to node if able to select new node
	public bool SelectNode(GameObject node)
	{
		SimpleNode otherNode = node.GetComponent<SimpleNode>();

		// First node of game
		if (selectedNodeObject == null)
		{
			selectedNodeObject = node;
			selectedNode = selectedNodeObject.GetComponent<SimpleNode>();

			// Make sure it is the start node
			if (selectedNode.GridLocation != startNode)
			{
				selectedNodeObject = null;
				selectedNode = null;
				return false;
			}

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
			// Left and right movement
			if (selectedNode.Type == "SIMPLE")
			{
				// Node that is already selected
				if (otherNode.GridLocation == selectedNode.GridLocation)
				{
					cursor.transform.position = selectedNode.transform.position;
					return false;
				}

				// UP DOWN
				if (otherNode.GridLocation == (selectedNode.GridLocation + height) ||
					(otherNode.GridLocation == (selectedNode.GridLocation - height)))
					{
						
						selectedNodeObject = node;
						selectedNode = node.GetComponent<SimpleNode>();
						cursor.transform.position = selectedNode.transform.position;

						// Add new point to line renderer
						line.AddPoint(selectedNodeObject.transform.position);
						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);

						Debug.Log(selectedNode.GridLocation.ToString());
						return true;
					}
				// LEFT RIGHT
				if (otherNode.GridLocation == (selectedNode.GridLocation +1) ||
					otherNode.GridLocation == (selectedNode.GridLocation -1))
				{
						selectedNodeObject = node;
						selectedNode = node.GetComponent<SimpleNode>();
						cursor.transform.position = selectedNode.transform.position;

						// Add new point to line renderer
						line.AddPoint(selectedNodeObject.transform.position);
						tempLine.ClearLine();
						tempLine.AddPoint(selectedNodeObject.transform.position);

						Debug.Log(selectedNode.GridLocation.ToString());
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
