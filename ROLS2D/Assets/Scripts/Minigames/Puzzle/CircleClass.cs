using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleClass: MonoBehaviour {

	protected bool isTouchable;
	protected int hitCount;
	protected int hitMax;
	protected int gridIndex;

	public bool IsTouchable{
		get{ return isTouchable; }
		set{ isTouchable = value; }
	}

	public int HitCount{
		get{return hitCount;}
		set{hitCount = value;}
	}

	public int GridIndex{
		get{ return gridIndex; }
		set{ gridIndex = value; }
	}

	public void SetUpCircle(int hit, int index){
		hitMax = hit;
		hitCount = hit; 
		gridIndex = index;
	}

	public void Reset(){
		hitCount = hitMax;
	}

}
