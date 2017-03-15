using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTest : MonoBehaviour {
	public bool isSwiping;
	private Vector2 startPos, endPos; 
	public GameObject startObj, endObj;
	private LineRenderer lr; 
	private int lineCounter = 0;

	RaycastHit2D hit; 

	// Use this for initialization
	void Start () {
		lr = GameObject.Find ("LineDisplay").GetComponent<LineRenderer>();
		if (lr == null)
			Debug.Log ("ERROR - CANNOT FIND LINE DISPLAY GAMEOBJECT IN SCENE");


		startObj = null;
		endObj = null;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		#if UNITY_EDITOR
		CheckMouseSwipe();
		#endif
	}
		
	void CheckMouseSwipe(){
		if (Input.GetMouseButtonDown (0)) {
			//startPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			isSwiping = true; 
		}

		if (isSwiping) {
			//endPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			ObjDetector ();
		}

		if (Input.GetMouseButtonUp (0)) {
			isSwiping = false;
			startObj = null;
			endObj = null;
			lr.numPositions = 0;
			lineCounter = 0;
		}
	}

	void ObjDetector(){
		hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.down);
		if (hit.collider != null) {
			if (startObj == null) {
				lineCounter += 1;
				lr.numPositions = lineCounter;
				startObj = hit.collider.gameObject;
				lr.SetPosition (0, startObj.transform.localPosition);

			}
			else if (startObj != null & endObj != null & endObj != hit.collider.gameObject) {
				endObj = hit.collider.gameObject;
				if (Vector3.Distance (startObj.transform.position, endObj.transform.position) == 2) {
					lineCounter += 1;
					startObj = endObj;
					lr.numPositions = lineCounter;
					int index = lineCounter - 1;
					lr.SetPosition (index, endObj.transform.localPosition);
				} else
					endObj = null;
				Debug.Log ("START OBJ: " + startObj.name);
				Debug.Log ("END OBJ: " + endObj.name);

			}
			else if (startObj != null & endObj == null && startObj != hit.collider.gameObject) {


				endObj = hit.collider.gameObject;
				if (Vector3.Distance (startObj.transform.position, endObj.transform.position) == 2) {
					lineCounter += 1;
					lr.numPositions = lineCounter;
					lr.SetPosition (1, endObj.transform.localPosition);
				}
				else
					endObj = null;


			}
				
		}
	}
}
