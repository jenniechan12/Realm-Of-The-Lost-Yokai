using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{

	Object BASE_CIRCLE;

	GameObject[] circles;

	Vector3[] circlesPos;
	Vector3 tempCirclePos;
	float tempX, tempY;

	private Color[] colorList;
	int row, col, count, maxCount;
	public int startPoint, endPoint;

	// Line Render
	private LineRenderer lr;
	private int lineCounter = 0; 

	// User's Input
	private bool isSwiping, isGameOVer = false;
	public GameObject startObj, endObj;
	RaycastHit2D hit;

	void Awake ()
	{
		// Line Render
		lr = GameObject.Find("LineDisplay").GetComponent<LineRenderer>();
		if (lr == null)
			Debug.Log ("ERROR - CANNOT FIND LINE DISPLAY GAMEOBJECT IN SCENE");

		colorList = new Color[]{ Color.black, Color.red, Color.blue };
		row = 3;
		col = 3; 
		maxCount = row * col;
		circles = new GameObject[maxCount];
		circlesPos = new Vector3[maxCount];

	}

	// Use this for initialization
	void Start ()
	{
		BASE_CIRCLE = Resources.Load ("Prefabs/Circle");
		SetCircle ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		CheckMouseSwipe ();
	}

	// *********************************************************************************************
	// SetCircle() Function - Create the puzzle layout
	// *********************************************************************************************
	void SetCircle ()
	{

		// Calculate the Start and End Point
		startPoint = (Mathf.FloorToInt (Random.Range (0, 9) / 2)) * 2;
		endPoint = (Mathf.FloorToInt (Random.Range (0, 9) / 2)) * 2;
		while (endPoint == startPoint) {
			endPoint = (Mathf.FloorToInt (Random.Range (0, 9) / 2)) * 2;
		}

		count = 0; 

		// Create the Level 
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {

				// Calculate the Circle's Position
				tempX = (i * 2) - 2;
				tempY = (j * -2) + 2;
				tempCirclePos = new Vector3 (tempX, tempY, 0);

				GameObject tempCircle;
				tempCircle = Instantiate (BASE_CIRCLE, tempCirclePos, Quaternion.identity) as GameObject;

				CircleClass ccScript = tempCircle.GetComponent<CircleClass> ();
				ccScript.SetUpCircle (1, count);
				tempCircle.GetComponent<SpriteRenderer> ().color = colorList [ccScript.HitCount];

				circles [count] = tempCircle;
				circlesPos [count] = tempCirclePos;
				count++;
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
			startObj = null;
			endObj = null;
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
			if (startObj == null) {
				if (tempCC.GridIndex == startPoint) {
					startObj = hit.collider.gameObject;
					HitCircle (startObj);
				} else
					startObj = null;
			} 

			// When end object is null, calculate end object
			else if (startObj != null && endObj == null && startObj != hit.collider.gameObject) {
				if (tempCC.GridIndex != endPoint) {
					if (tempCC.HitCount > 0 && CheckCircleAdjacent (startObj, hit.collider.gameObject)) {
						endObj = hit.collider.gameObject;
						HitCircle (endObj);							
					} else
						endObj = startObj;
				} else {
					endObj = hit.collider.gameObject;
					HitCircle (endObj);
					CalculateWin ();
					isGameOVer = true;
					Invoke ("ResetLevel", 1.5f);

				}
			} 

			// When start object and end object are not null, do this
			else if (startObj != null && endObj != null && endObj != hit.collider.gameObject) {
				if (tempCC.GridIndex != endPoint) {
					startObj = endObj;

					if (tempCC.HitCount > 0 && CheckCircleAdjacent (endObj, hit.collider.gameObject)) {
						//startObj = endObj;
						endObj = hit.collider.gameObject;
						HitCircle (endObj);
					} else
						endObj = startObj;
				} else {
					if (CheckCircleAdjacent (endObj, hit.collider.gameObject)) {
						endObj = hit.collider.gameObject;
						HitCircle (endObj);
						CalculateWin ();
						isGameOVer = true;
						Invoke ("ResetLevel", 1.5f);
					}
				}

			}
		}
	}

	bool CheckCircleAdjacent(GameObject obj1, GameObject obj2){
		CircleClass obj1cc = obj1.GetComponent<CircleClass> ();
		CircleClass obj2cc = obj2.GetComponent<CircleClass> ();


		if ((Vector3.Distance(obj1.transform.position, obj2.transform.position) == 2) &&
			(obj2cc.GridIndex == obj1cc.GridIndex + 1 ||
			obj2cc.GridIndex == obj1cc.GridIndex - 1 ||
			obj2cc.GridIndex == obj1cc.GridIndex + 3 ||
			obj2cc.GridIndex == obj1cc.GridIndex - 3
			)){

			return true;
		}
		else{ 
			return false;
		}
	}

	void CalculateWin ()
	{
		int damageCount = 0;

		foreach (GameObject circle in circles) {
			CircleClass tempCCScript = circle.GetComponent<CircleClass> ();
			if (tempCCScript.HitCount == 0) {
				damageCount++;
			}
		}

		Debug.Log ("You dealt " + damageCount + " damage points");

	}

	void ResetLevel ()
	{
		foreach (GameObject circle in circles) {
			CircleClass tempCC = circle.GetComponent<CircleClass> ();
			tempCC.Reset ();

			circle.GetComponent<SpriteRenderer> ().color = colorList [tempCC.HitCount];
		}

		// Reset Line Renderer
		lineCounter = 0;
		lr.numPositions = lineCounter;

	}
}




