using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	List<Vector3> pointList;
	LineRenderer lineRenderer;

	// Use this for initialization
	void Start () 
	{
		lineRenderer = GetComponent<LineRenderer>();
		pointList = new List<Vector3>();
		lineRenderer.sortingLayerName = "Line";
	}


	// Render the line with new points
	public void UpdateLine()
	{
		lineRenderer.numPositions = pointList.Count;
		Vector3[] lineArray = new Vector3[pointList.Count];
		for (int i = 0; i < pointList.Count; i++)
		{
			lineArray[i] = pointList[i];
		}
		lineRenderer.SetPositions(lineArray);
	}


	// Add a new point to the line renderer list and update
	public void AddPoint(Vector3 point)
	{
		pointList.Add(point);
		UpdateLine();
	}

	public void ClearLine()
	{
		pointList.Clear();
		lineRenderer.numPositions = 0;
	}
}
