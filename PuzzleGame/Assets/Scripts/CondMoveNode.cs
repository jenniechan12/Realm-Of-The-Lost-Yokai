using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondMoveNode : MonoBehaviour {

	List<Vector3> wayPoints;
	int currentPointIndex;
	Vector3 destination;
	float moveSpeed;
	Vector3 moveDir;
	bool active;

	void Awake()
	{
		wayPoints = new List<Vector3>();
		destination = Vector3.zero;
		moveDir = Vector3.zero;
		currentPointIndex = 0;
		active = false;
	}

	void Update()
	{
		if (active)
		{
			transform.Translate(moveDir * Time.deltaTime);
			// If node has reached its destination
			if (Vector3.Magnitude(transform.position - destination) < 0.1f)
			{
				transform.position = destination;
				active = false;
				UpdateDestination();
			}
		}
	}

	void UpdateDestination()
	{
		currentPointIndex++;
		// Wrap back to front of list
		if (currentPointIndex >= wayPoints.Count)
			currentPointIndex = 0;

		destination = wayPoints[currentPointIndex];
		moveDir = destination - transform.position;
		moveDir.Normalize();
		moveDir *= moveSpeed;
	}

	// Use this for initialization
	void Start () {
		destination = wayPoints[currentPointIndex];
		moveDir = destination - transform.position;
		moveDir.Normalize();
		moveDir *= moveSpeed;
	}

	public void AddWayPoint(Vector3 point)
	{
		wayPoints.Add(point);
	}

	public void SetupNode(float speed)
	{
		moveSpeed = speed;
	}

	public void Activate()
	{
		active = true;
	}
}

