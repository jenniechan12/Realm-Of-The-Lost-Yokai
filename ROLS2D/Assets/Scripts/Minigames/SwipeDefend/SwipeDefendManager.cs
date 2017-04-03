using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDefendManager : MonoBehaviour {

	GameObject battleManagerObj;
	BattleManager battleManager;
	Object ENEMY;
	GameObject player;
	Object PLAYER;
	Object SWIPE_SLASH;
	GameObject swipeSlash;

	Vector3 playerPosition;
	List<GameObject> spawnLocations;
	List<GameObject> enemyList;

	float spawnTimer; 
	float gameTimer;
	float gameDuration;
	float spawnTime;

	float easySpeed;

	int numHit;		// number that were destroyed by player swiping
	int numMissed; // number that hit player sprite

	int enemyDamage;	// Enemy wont take any damage this game
	int playerDamage;	// How much damage the player takes

	// Use this for initialization
	void Start () {
		battleManagerObj = GameObject.Find("BattleManager");
		if (battleManagerObj)
			battleManager = battleManagerObj.GetComponent<BattleManager>();
		if (battleManagerObj == null)
			Debug.Log("Error finding battle manager obj in MinigameTest");
		if (battleManagerObj == null)
			Debug.Log("Error finding battle manager component in MinigameTest");

		ENEMY = Resources.Load("Prefab/Minigames/SwipeDefend/SwipeEnemy") as Object;
		PLAYER = Resources.Load("Prefab/Minigames/SwipeDefend/SwipePlayer") as Object;
		SWIPE_SLASH = Resources.Load("Prefab/Minigames/SwipeDefend/SwipeSlash") as Object;

		// Instantiate objects
		playerPosition = Vector3.zero;
		player = Instantiate(PLAYER, playerPosition, Quaternion.identity) as GameObject;

		swipeSlash = Instantiate(SWIPE_SLASH, Vector3.zero, Quaternion.identity) as GameObject;
		swipeSlash.SetActive(false);

		spawnTimer = 0;
		spawnTime = 1.0f;
		gameTimer = 0;
		gameDuration = 10.0f;

		easySpeed = 1.0f;
		numHit = 0;
		numMissed = 0;

		enemyDamage = 0;
		playerDamage = 0;

		// Set spawn points
		// Put difficulty logic here

		spawnLocations = new List<GameObject>();
		enemyList = new List<GameObject>();

		foreach (Transform child in transform)
		{
			if (child.tag == "SwipeSpawn")
			{
				spawnLocations.Add(child.gameObject);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		spawnTimer += Time.deltaTime;
		gameTimer += Time.deltaTime;

		if (spawnTimer >= spawnTime)
		{
			spawnTimer = 0;
			SpawnEnemy();
		}

		if (gameTimer >= gameDuration)
		{
			DestroyObjects();
			battleManager.MinigameEnd(enemyDamage, playerDamage);
			Destroy(gameObject);
		}

		CheckSwipeSlash();
	}

	void CheckSwipeSlash()
	{
		if (Input.GetMouseButtonDown (0))
		{
    		swipeSlash.SetActive(true);
		}
 
		if (Input.GetMouseButton (0))
		{
			float mouseX = Input.mousePosition.x;
			float mouseY = Input.mousePosition.y;
			Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3 (mouseX, mouseY, 5));
			position.z = 0;
			swipeSlash.transform.position = position;
		}
 
		if (Input.GetMouseButtonUp (0))
		{
  		  swipeSlash.SetActive(false);
		}
	}

	void SpawnEnemy()
	{
		// Set up random number generator
		int rand = Random.Range(0, spawnLocations.Count - 1);
		Vector3 direction = playerPosition - spawnLocations[rand].transform.position;

		// Create enemy
		GameObject tempEnemy = Instantiate(ENEMY, spawnLocations[rand].transform.position, Quaternion.identity) as GameObject;
		tempEnemy.name = ENEMY.name;
		enemyList.Add(tempEnemy);

		// Set enemy's variables
		SwipeEnemy swipeEnemy = tempEnemy.GetComponent<SwipeEnemy>();
		swipeEnemy.SetupEnemy(direction, easySpeed);
	}

	void DestroyObjects()
	{
		foreach(GameObject enemy in enemyList)
		{
			Destroy(enemy);
		}
		if (player != null)
			Destroy(player);
	}

	public void RemoveEnemy(GameObject enemy)
	{
		enemyList.Remove(enemy);
	}

	public void NumHit()
	{
		numHit++;
	}

	public void NumMissed()
	{
		numMissed++;
		playerDamage++;
	}
}
