using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolArea : MonoBehaviour {
	public Transform[] patrolsPos;	// Array of Enemy's Patrol Position
	public float moveSpeed; 		// Enemy's movement speed
	int currentIndex;


	// Use this for initialization
	void Start () {
		currentIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector2.MoveTowards (transform.position, patrolsPos [currentIndex].position, moveSpeed * Time.deltaTime);

	}
}
