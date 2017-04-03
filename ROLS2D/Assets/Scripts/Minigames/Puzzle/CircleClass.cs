using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleClass: MonoBehaviour
{

	protected string circleType;
	protected int hitCount;
	protected int hitMax;
	protected int gridRow;
	protected int gridCol;

	public string CircleType {
		get{ return circleType; }
		set{ circleType = value; }
	}

	public int HitCount {
		get{ return hitCount; }
		set{ hitCount = value; }
	}

	public int GridRow {
		get{ return gridRow; }
		set{ gridRow = value; }
	}

	public int GridCol {
		get{ return gridCol; }
		set{ gridCol = value; }
	}

	public void SetUpCircle (int hit, int row, int col)
	{
		hitMax = hit;
		hitCount = hit; 
		gridRow = row;
		gridCol = col;
	}

	public void Reset ()
	{
		hitCount = hitMax;
	}

}
