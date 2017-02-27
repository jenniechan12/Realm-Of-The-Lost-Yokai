using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportPortal : MonoBehaviour {
	public Transform portal;
	public Transform cameraPos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log ("Time to teleport you");
		other.transform.position = portal.position;
		Camera.main.transform.position = cameraPos.position;
	}
}
