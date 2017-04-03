using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeEnemy : MonoBehaviour {

	SwipeDefendManager swipeDefendManager;

	Vector3 moveDirection;
	float moveSpeed;

	// Use this for initialization
	void Awake () {
		moveDirection = Vector3.zero;
		moveSpeed = 0;
	}

	void Start()
	{
		swipeDefendManager = GameObject.Find("SwipeDefend").GetComponent<SwipeDefendManager>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(moveDirection * Time.deltaTime);
	}

	// Set values (called by swipe manager)
	public void SetupEnemy(Vector3 direction, float speed)
	{
		moveDirection = direction;
		moveSpeed = speed;
		moveDirection *= moveSpeed;
	}

	// If we hit the player sprite
	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			// Remove from child list of swipe manager
			swipeDefendManager.RemoveEnemy(gameObject);
			// Tell manager enemy hit the player
			swipeDefendManager.NumMissed();
			Destroy(gameObject);
		}
		else if (other.tag == "SwipeSlash")
		{
			// Remove from child list of swipe manager
			swipeDefendManager.RemoveEnemy(gameObject);
			// Tell manager enemy was destroyed by swipe
			swipeDefendManager.NumHit();
			Destroy(gameObject);
		}
	}
}
