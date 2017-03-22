using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingNode : MonoBehaviour {

	List<Vector3> wayPoints;
	Vector3 destination;

	float moveSpeed;

	void Awake()
	{
		wayPoints = new List<Vector3>();
		destination = Vector3.zero;
	}

	// Use this for initialization
	void Start () {
		destination = wayPoints[0];
	}

	public void AddWayPoint(Vector3 point)
	{
		wayPoints.Add(point);
	}

	public void SetupNode(float speed)
	{
		moveSpeed = speed;
	}
}
