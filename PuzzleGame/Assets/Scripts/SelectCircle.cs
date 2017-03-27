using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCircle : MonoBehaviour {

	float maxSize;
	float minSize;
	float speed;


	// Use this for initialization
	void Start () 
	{	
		maxSize = 1.7f;
		minSize = 1.2f;
		speed = 1.01f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (transform.localScale.x <= minSize)
			speed = 1.01f;
		if (transform.localScale.x >= maxSize)
			speed = 0.99f;

		transform.localScale *= speed;
	}
}
