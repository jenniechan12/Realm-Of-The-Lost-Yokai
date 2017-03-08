using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class PlayerManager : MonoBehaviour
{
	public static PlayerManager instance = null;

	// Player Information
	public string _playerName;
	public string _playerClass;
	public int _playerLevel;
	public int _playerExp;
	public int _playerHP;
	public int _playerAP;
	public int _playerVit;
	public int _playerStr;
	public int _playerDef;
	public int _playerLuck;
	public float _playerPosX;
	public float _playerPosY;
	public float _playerPosZ;

	// Level and Experience 
	private int MAX_EXP; 
	private List<int> expList = new List<int> ();

	// Attribute Points
	public GameObject playerStatMenu;
	public Text _vitAP, _strAP, _defAP, _luckAP;
	private int _vitCount = 0; 
	private int _strCount = 0;
	private int _defCount = 0;
	private int _luckCount = 0;


	void Awake ()
	{
		// Check if instance is already exist
		if (instance == null)

			// if not, set instance to this
			instance = this;

		// If instance already exist and it's not this:
		else if (instance != this)

			// Destroy gameobject
			Destroy (gameObject);

		// Load Player File
		LoadPlayerFile ();

		// Get EXP List
		GetEXPList();

		DontDestroyOnLoad (gameObject);

	}

	void Start()
	{
		// Set MAX EXP based on player level
		if (expList.Count > 0 && _playerLevel < expList.Count) {
			MAX_EXP = expList [_playerLevel];
		}
	}
		
	public TextAsset playerData;
	public TextAsset expData;

	// xml file for player data
	public GameObject player, playerManager;
	XmlDocument playerDocument = new XmlDocument ();

	//************************************************************
	// FUNCTIONS FOR LEVEL AND EXP
	//************************************************************

	// GainLevel() Function - when player level up, gain a level, 
	// gain 3 AP, new MAX_EXP
	private void GainLevel(){
		_playerLevel++;
		_playerAP += 3;
		if (expList.Count > 0 && _playerLevel < expList.Count) {
			MAX_EXP = expList [_playerLevel];
		}
	}

	// GainEXP(int) Function - gain exp and calculate player's
	// current exp 
	private void GainEXP(int points){
		_playerExp += points;
		while(_playerExp >= MAX_EXP) {
			_playerExp -= MAX_EXP;
			GainLevel ();
		}
	}

	// DisplayEXP() Function - display player's current level 
	// and exp 
	public void DisplayEXP(){
		Debug.Log ("Level: " + _playerLevel);
		Debug.Log ("Experience: " + _playerExp);
	}

	//************************************************************
	// FUNCTIONS FOR PLAYER'S STATS
	//************************************************************

	// EnableStatMenu() Function - display player's stat menu
	public void EnableStatMenu(){
		playerStatMenu.SetActive (true);
	}

	// DisableStatMenu() Function - hide player's stat menu
	public void DisableStatMenu(){
		playerStatMenu.SetActive (false);
	}

	// ConfirmAP() Function - finalize attribute points (AP) and 
	// calculate final stats
	public void ConfirmAP(){
		AddVitality (_vitCount);
		AddStrength (_strCount);
		AddDefense (_defCount);
		AddLuck (_luckCount);

		// Restart counter
		_vitCount = 0;
		_strCount = 0;
		_defCount = 0;
		_luckCount = 0;

		// Display Count
		_vitAP.text = _vitCount.ToString();
		_strAP.text = _strCount.ToString ();
		_defAP.text = _defCount.ToString ();
		_luckAP.text = _luckCount.ToString ();
	}

	// Increase and Decrease Button Functions - displays a counter
	// for each stats based on player's input and AP
	public void IncreaseVitality(){
		if (_playerAP > 0) {
			_playerAP--;
			_vitCount++;
		}
		_vitAP.text = _vitCount.ToString ();
	}
	public void DecreaseVitality(){
		if (_vitCount > 0){
			_vitCount--;
			_playerAP++;
		}
		_vitAP.text = _vitCount.ToString ();
	}
	public void IncreaseStrength(){
		if (_playerAP > 0) {
			_playerAP--;
			_strCount++;
		}
		_strAP.text = _strCount.ToString ();
	}
	public void DecreaseStrength(){
		if (_strCount > 0) {
			_strCount--;
			_playerAP++;
		}

		_strAP.text = _strCount.ToString ();
	}
	public void IncreaseDefense(){
		if (_playerAP > 0) {
			_playerAP--;
			_defCount++;
		}
		_defAP.text = _defCount.ToString ();
	}
	public void DecreaseDefense(){
		if (_defCount > 0) {
			_defCount--;
			_playerAP++;
		}

		_defAP.text = _defCount.ToString ();
	}
	public void IncreaseLuck(){
		if (_playerAP > 0) {
			_playerAP--;
			_luckCount++;
		}
		_luckAP.text = _luckCount.ToString ();
	}
	public void DecreaseLuck(){
		if (_luckCount > 0) {
			_luckCount--;
			_playerAP++;
		}

		_luckAP.text = _luckCount.ToString ();
	}

	private void AddVitality(int point){
		_playerVit += point;
	}
	private void AddStrength(int point){
		_playerStr += point;
	}
	private void AddDefense(int point){
		_playerDef += point;
	}
	private void AddLuck(int point){
		_playerLuck += point;
	}


	//************************************************************
	// LOAD PLAYER FILE FUNCTION
	//************************************************************
	// Load Player's Data from XML Database
	// TODO: Load file through path instead of gameobject
	public void LoadPlayerFile ()
	{
		Debug.Log ("Starting to Load Player File"); 
		playerDocument.LoadXml (playerData.text);

		XmlNode root = playerDocument.FirstChild.FirstChild;
		foreach (XmlNode content in root.ChildNodes) {
			switch (content.Name) {
			case("PlayerName"):
				_playerName = content.InnerText;
				break;
			case("PlayerClass"):
				_playerClass = content.InnerText;
				break;
			case("PlayerLevel"):
				_playerLevel = int.Parse (content.InnerText);
				break;
			case("PlayerExperience"):
				_playerExp = int.Parse (content.InnerText);
				break;
			case("PlayerHP"):
				_playerHP = int.Parse (content.InnerText);
				break;
			case("PlayerAP"):
				_playerAP = int.Parse (content.InnerText);
				break;
			case("PlayerVitality"):
				_playerVit = int.Parse (content.InnerText);
				break;
			case("PlayerStrength"):
				_playerStr = int.Parse(content.InnerText);
				break;
			case("PlayerDefense"):
				_playerDef = int.Parse (content.InnerText);
				break;
			case("PlayerLuck"):
				_playerLuck = int.Parse (content.InnerText);
				break;
			case("PlayerPosX"):
				_playerPosX = XmlConvert.ToSingle (content.InnerText);
				break;
			case("PlayerPosY"):
				_playerPosY = XmlConvert.ToSingle (content.InnerText);
				break;
			case("PlayerPosZ"):
				_playerPosZ = XmlConvert.ToSingle (content.InnerText);
				break;

			}
		}


		Debug.Log ("Finish Loading Player File");
	}

	//************************************************************
	// SAVE PLAYER FILE FUNCTION
	//************************************************************
	// Save Player's Latest Data and store it in XML Database
	public void SavePlayerFile ()
	{

		// Check for player in scene
		if (player != null) {
			XmlNode root = playerDocument.FirstChild.FirstChild;	// set root to "Info" from Player Stats xml file

			foreach (XmlNode content in root.ChildNodes) {
				switch (content.Name) {
				case("PlayerName"):
					content.InnerText = _playerName.ToString ();
					break;
				case("PlayerClass"):
					content.InnerText = _playerClass.ToString ();
					break;
				case("PlayerLevel"):
					content.InnerText = _playerLevel.ToString ();
					break;
				case("PlayerExperience"):
					content.InnerText = _playerExp.ToString ();
					break;
				case("PlayerHP"):
					content.InnerText = _playerHP.ToString ();
					break;
				case("PlayerAP"):
					content.InnerText = _playerAP.ToString ();
					break;
				case("PlayerVitality"):
					content.InnerText = _playerVit.ToString ();
					break;
				case("PlayerStrength"):
					content.InnerText = _playerStr.ToString ();
					break;
				case("PlayerDefense"):
					content.InnerText = _playerDef.ToString ();
					break;
				case("PlayerLuck"):
					content.InnerText = _playerLuck.ToString ();
					break;
				case("PlayerPosX"):
					content.InnerText = player.transform.localScale.x.ToString ();
					break;
				case("PlayerPosY"):
					content.InnerText = player.transform.localScale.y.ToString ();
					break;
				case("PlayerPosZ"):
					content.InnerText = player.transform.localScale.z.ToString ();
					break;
				}
			}

			// Save data to xml file
			Debug.Log ("Finish Saving Data... Save to File Now");
			playerDocument.Save (Application.dataPath.ToString() + "/XML/PlayerStats.xml");
			Debug.Log ("File is saved.");


		} else
			Debug.Log ("ERROR - Cannot find player -> Cannot Save Player File");
	}

	//************************************************************
	// GET EXP LIST FUNCTION
	//************************************************************
	// Load and Store Max Exp per Level in expList
	private void GetEXPList(){
		playerDocument.LoadXml (expData.text);
		XmlNode root = playerDocument.FirstChild;

		foreach (XmlNode content in root.ChildNodes) 
		{
			expList.Add (int.Parse (content.InnerText));
		}

	}
}

