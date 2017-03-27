using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	private enum MenuState {SYSTEM, INVENTORY, STATS, CLOSED};
	private MenuState currentState;

	private PlayerInventory playerInventory;
	private BattleManager battleManager;
	private GameManager gameManager;

	private GameObject systemButton;
	private GameObject inventoryButton;
	private GameObject statsButton;
	private GameObject exitButton;
	private GameObject openButton;

	// Our current display state
	bool displaying;

	void Awake () 
	{
		currentState = MenuState.CLOSED;
		displaying = false;
	}

	void Start()
	{
		playerInventory = GameObject.Find("PlayerInventory").GetComponent<PlayerInventory>();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		SetButtons();
	}

	void SetButtons()
	{
		 systemButton = GameObject.Find("SystemButton");
		 inventoryButton = GameObject.Find("InventoryButton");
		 statsButton = GameObject.Find("StatsButton");
		 exitButton = GameObject.Find("ExitButton");
		 openButton = GameObject.Find("OpenButton");

		 Button button;
		 button = systemButton.GetComponent<Button>();
		 button.onClick.AddListener(SystemButtonClick);
		 systemButton.SetActive(false);

		 button = inventoryButton.GetComponent<Button>();
		 button.onClick.AddListener(InventoryButtonClick);
		 inventoryButton.SetActive(false);

		 button = statsButton.GetComponent<Button>();
		 button.onClick.AddListener(StatsButtonClick);
		 statsButton.SetActive(false);

		 button = exitButton.GetComponent<Button>();
		 button.onClick.AddListener(ExitButtonClick);
		 exitButton.SetActive(false);


		 button = openButton.GetComponent<Button>();
		 button.onClick.AddListener(OpenButtonClick);
		 if (gameManager.Battling())
		 {
			openButton.SetActive(false);
		}
		else
		{
			openButton.SetActive(true);
		}
		
	}


	void SwitchMenus(MenuState nextState)
	{
		//systemManager.Hide();
		//equipManager.Hide();
		playerInventory.Hide();
		if (nextState == MenuState.INVENTORY)
			playerInventory.Display();
		//else if (nextState == MenuState.STATS)
		//	equipManager.Display();
		//else if (nextState == MenuState.SYSTEM)
		// systemManager.Display();
	}

	public void InventoryButtonClick()
	{
		SwitchMenus(MenuState.INVENTORY);
	}

	public void StatsButtonClick()
	{
		SwitchMenus(MenuState.STATS);
	}

	public void SystemButtonClick()
	{
		SwitchMenus(MenuState.SYSTEM);
	}

	public void ExitButtonClick()
	{
		displaying = false;

		// Update buttons
		if (gameManager.Battling())
			openButton.SetActive(false);
		else
			openButton.SetActive(true);

		inventoryButton.SetActive(false);
		systemButton.SetActive(false);
		statsButton.SetActive(false);
		currentState = MenuState.CLOSED;

		// Hide all UI
		playerInventory.Hide();
	}

	public void OpenButtonClick()
	{
		// Update buttons
		displaying = true;
		openButton.SetActive(false);
		inventoryButton.SetActive(true);
		systemButton.SetActive(true);
		statsButton.SetActive(true);

		currentState = MenuState.INVENTORY;

		// Display inventory by default
		playerInventory.Display();
	}

	public bool Displaying
	{
		get {return displaying;}
		set {displaying = value;}
	}
}
