using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	Vector2 startPos, endPos, currentSwipe;
	float currentSpeed, maxSpeed = 3.0f, acceleration = 1f;
	bool isMoving = false;
	Animator anim;
	Rigidbody2D playerRB;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		playerRB = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		CheckMouseSwipe ();
	}

	void CheckMouseSwipe(){
		if (Input.GetMouseButtonDown (0)) {
			// Save start touch position
			startPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			isMoving = true;
		}

		if (isMoving) {
			// Save end touch position
			endPos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

			// Calculate swipe movement
			currentSwipe = new Vector2 (endPos.x - startPos.x, endPos.y - startPos.y);

			// Normalize the current swipe movement
			currentSwipe.Normalize ();

			MovementSpeed ();

			//  Swipe Left 
			if (currentSwipe.x < 0 && (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)) {
				Debug.Log ("Player Swipe Left");
				anim.Play ("WALK_LEFT");
				playerRB.velocity = new Vector2 (-0.2f*currentSpeed, playerRB.velocity.y);
			}

			// Swipe Right
			if (currentSwipe.x > 0 && (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)) {
				Debug.Log ("Player Swipe Right");
				anim.Play ("WALK_RIGHT");
				playerRB.velocity = new Vector2 (0.2f*currentSpeed, playerRB.velocity.y);


			}

			// Swipe Up
			if (currentSwipe.y > 0 && (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)) {
				Debug.Log ("Player Swipe Up");
				anim.Play ("WALK_UP");
				playerRB.velocity = new Vector2 (playerRB.velocity.x, 0.2f*currentSpeed);

			}

			// Swipe Down
			if (currentSwipe.y < 0 && (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)) {
				Debug.Log ("Player Swipe Down");
				anim.Play ("WALK_DOWN");
				playerRB.velocity = new Vector2 (playerRB.velocity.x, -0.2f*currentSpeed);

			}
		}

		if (Input.GetMouseButtonUp (0)) {
			isMoving = false;

			// Play Idle Animation
			anim.Play ("IDLE");

			currentSpeed = 0;
			playerRB.velocity = Vector2.zero;
		}

	}

	void OnCollision2DEnter(Collision other){
		Debug.Log (other.collider.name);
	}

	void MovementSpeed(){
		currentSpeed += acceleration * Time.deltaTime;

		if (currentSpeed > maxSpeed) {
			currentSpeed = maxSpeed;
		}
	}
}
