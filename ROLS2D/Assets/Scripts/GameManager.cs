using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{

	private enum GameStates {MAIN_MENU, PLAYING};
	private enum PlayStates {BATTLING, PUZZLING, MAIN};
	private PlayStates currentPlayState;


	void Awake () 
	{
		currentPlayState = PlayStates.MAIN;
	}
	
	public bool Battling()
	{
		if (currentPlayState == PlayStates.BATTLING)
			return true;
		else
			return false;
	}

	public void BattleTrigger()
	{
		if (currentPlayState != PlayStates.BATTLING)
			currentPlayState = PlayStates.BATTLING;
		else
			currentPlayState = PlayStates.MAIN;
	}
}
