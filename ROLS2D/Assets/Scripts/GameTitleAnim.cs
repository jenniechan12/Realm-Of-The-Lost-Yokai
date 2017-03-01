using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTitleAnim: MonoBehaviour {
	public Text startText;
	private Image titleIMG;
	private float time, total_time = 5f;

	// Use this for initialization
	void Start () {
		titleIMG = GetComponent<Image> ();
		time = 0;
	}

	void Update(){
		if (time <= total_time) {
			time += Time.deltaTime;
			titleIMG.fillAmount = time / total_time;
		} else {
			Invoke ("EnableText", 1f);
		}

		if (startText.enabled) {
			if (Input.GetMouseButton (0)) {
				//Debug.Log ("PLAY GAME");
				SceneManager.LoadScene(1);
			}
		}
	}

	void EnableText(){
		startText.enabled = true;
	}


}
