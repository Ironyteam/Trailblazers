using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {
    
    public const int MAX_NUM_OF_PLAYERS = 6;
	public const int LOCAL_PLAYER = 0;

    public Button BuildRoad;
    public Button BuildSettlement;
    public Button BuildCity;
    public Button BuyArmy;
    public Button Attack;
    public Button Market;
    public Button EndTurn;
    public Image DiceImage;
    public Text DiceText,
				inGamePopupText;
		   	    
    public bool GameCanvasEnabled = true;

    [System.Serializable]
    public class playerIndicatorClassLocal
    {
		public bool largestArmyIndicator = false;
		public bool longestRoadIndicator = false;
		public string characterPictureName;
		public int uiPosition;
		public string playerColourName;
    }

    [System.Serializable]
	public class screenElements
	{
		public Canvas playerCanvas;
		public Image largestArmyIndicator;
		public Image longestRoadIndicator;
		public Image characterPicture;
        public Image characterSelected;
        public Text armySizeDisplay;
		public Text longestRoadLengthDisplay;
		public Text victoryPoints;
		public Text playerName;
		public Image playerColour;
	}
	
	[System.Serializable]
	public class scoreboardElements
	{
		public Canvas ScoreboardCanvas;
		public Image  characterImage,
					  longestRoadIndicator,
					  largestArmyIndicator;
		public Text   characterName,
					  numOfConsecutiveRoads,
					  numOfArmies,
					  numOfVP;
	}

	public Dropdown chatDropdown;
    public InputField chatInput;
    public Text brickScore;
    public Text chatBox;
    public Text diceValue;
    public Text goldScore;
    public Text oreScore;
    public Text sheepScore;
    public Text wheatScore;
	public Text scoreBoardWinnerText;
    public Text woodScore;
    public Text dropLabel;
    public Canvas gameCanvas,
   	              escapeCanvas,
		          optionsCanvas,
				  shopCanvas,
				  barracksCanvas,
                  scoreboardCanvas;
           int currentPlayer  = 0,
 			   playerNumberCurrent = 0;
    
    public playerIndicatorClassLocal[] playerClassLocalArray = new playerIndicatorClassLocal[Constants.MaxPlayers];
	public screenElements[] screenElementsArray = new screenElements[Constants.MaxPlayers];
	public scoreboardElements[] scoreboardElementsArray = new scoreboardElements[Constants.MaxPlayers];
    // If changing this, Dont forget to change the values in the editor as well
	public string[] playerColourNames = new string[Constants.MaxPlayers] {"red box", "blue box", "purple box", "yellow box", "green box", "white box"};
    public GameBoard CurrentGameBoard;
    
    void Awake()
    {
		Destroy (GameObject.Find("ISLAND"));
		Destroy (GameObject.Find("Particle System"));
		Destroy (GameObject.Find("Directional light"));
		Destroy (GameObject.Find("Ocean"));
        CurrentGameBoard = GameObject.Find("Map").GetComponent<GameBoard>();
    }

	void Start()
	{
      for (int count = 0; count < BoardManager.numOfPlayers; count++)
        {
            playerClassLocalArray[count].characterPictureName = Characters.Names[characterSelect.selectedCharacters[count]];
            playerClassLocalArray[count].playerColourName = playerColourNames[count];
            playerClassLocalArray[count].uiPosition = count;
            screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite>(playerClassLocalArray[count].characterPictureName) as Sprite;
            screenElementsArray[count].playerColour.sprite = Resources.Load<Sprite>(playerClassLocalArray[count].playerColourName) as Sprite;
            screenElementsArray[count].largestArmyIndicator.enabled = false;
            screenElementsArray[count].longestRoadIndicator.enabled = false;
            screenElementsArray[count].characterSelected.enabled = false;
            scoreboardElementsArray[count].numOfVP = GameObject.Find("p" + (count + 1) + "NumOfVP").GetComponent<Text>();
            scoreboardElementsArray[count].numOfArmies = GameObject.Find("p" + (count + 1) + "NumOfArmies").GetComponent<Text>();
            scoreboardElementsArray[count].numOfConsecutiveRoads = GameObject.Find("p" + (count + 1) + "NumOfConsecutiveRoads").GetComponent<Text>();
            scoreboardElementsArray[count].characterName = GameObject.Find("p" + (count + 1) + "CharacterName").GetComponent<Text>();
            scoreboardElementsArray[count].characterImage = GameObject.Find("p" + (count + 1) + "CharacterImage").GetComponent<Image>();
            scoreboardElementsArray[count].largestArmyIndicator = GameObject.Find("p" + (count + 1) + "largestArmyIndicator").GetComponent<Image>();
            scoreboardElementsArray[count].longestRoadIndicator = GameObject.Find("p" + (count + 1) + "LongestRoadIndicator").GetComponent<Image>();
            scoreboardElementsArray[count].largestArmyIndicator.sprite = Resources.Load<Sprite>("yellow star") as Sprite;
            scoreboardElementsArray[count].longestRoadIndicator.sprite = Resources.Load<Sprite>("Red Star") as Sprite;
            scoreboardElementsArray[count].ScoreboardCanvas = GameObject.Find("player" + (count + 1) + "CanvasScoreboard").GetComponent<Canvas>();
        }

      // Set the current player highlighted
      if (NavigationScript.networkGame)
         screenElementsArray[playerClassLocalArray[CurrentGameBoard.CurrentPlayer].uiPosition].characterSelected.enabled = true;
      Debug.Log("Start: " + playerClassLocalArray[CurrentGameBoard.CurrentPlayer].uiPosition);

      chatDropdown = GameObject.Find("chatInputDropdown").GetComponent<Dropdown>(); //**********************
		inGamePopupText = GameObject.Find("popupTextGlobal").GetComponent<Text>();
        dropLabel = GameObject.Find("dropLabel").GetComponent<Text>();
        dropLabel.text = Constants.chatMessages[0];

		for(int count = 0; count < Constants.MaxChatMessages; count++)
			chatDropdown.options.Add(new Dropdown.OptionData(Constants.chatMessages[count]));

        gameCanvas.enabled = true;
        escapeCanvas.enabled = false;
        optionsCanvas.enabled = false;
        shopCanvas.enabled = false;
        barracksCanvas.enabled = false;
        scoreboardCanvas.enabled = false;
        DiceImage.enabled = false;
        DiceText.enabled = false;
		inGamePopupText.enabled = false;

        for (int count = 0; count < MAX_NUM_OF_PLAYERS; count++)
        {
            if (count < BoardManager.numOfPlayers)
            {
                screenElementsArray[count].playerCanvas.enabled = true;
                scoreboardElementsArray[count].ScoreboardCanvas.enabled = true;
            }
            else
            {
                scoreboardElementsArray[count].ScoreboardCanvas = GameObject.Find("player" + (count + 1) + "CanvasScoreboard").GetComponent<Canvas>();
                screenElementsArray[count].playerCanvas.enabled = false;
                scoreboardElementsArray[count].ScoreboardCanvas.enabled = false;
            }
        }

        if(NavigationScript.networkGame == true)
        {
          Debug.Log("Start: putting local player first");
          putLocalPlayerFirst(CurrentGameBoard.LocalPlayer);
        }
   }

   void Update ()
    {
        if (Input.GetKeyDown("escape"))
        {
			if(shopCanvas.enabled == true)
			{
				shopCanvas.enabled = false;
			}
            else
			{
				if(barracksCanvas.enabled == true)
				{
					barracksCanvas.enabled = false;
				}
				else
				{
		            if(escapeCanvas.enabled == false)
					{
		                escapeCanvas.enabled = true;
						optionsCanvas.enabled = false;
						barracksCanvas.enabled = false;
					}
					else
					{
						if(optionsCanvas.enabled == true)
						{
		                	escapeCanvas.enabled = true;
							optionsCanvas.enabled = false;
						}
						else
						{
							escapeCanvas.enabled = false;
						}
					}
				}
			}
        }

	    if (Input.GetKeyDown("s"))
		{
			if(shopCanvas.enabled == false)
			{
				shopCanvas.enabled = true;
				optionsCanvas.enabled = false;
				barracksCanvas.enabled = false;
				escapeCanvas.enabled = false;
			}
			else
				shopCanvas.enabled = false;
		}

		if (Input.GetKeyDown("b"))
		{
			if(barracksCanvas.enabled == false)
			{
				shopCanvas.enabled = false;
				optionsCanvas.enabled = false;
				escapeCanvas.enabled = false;
				barracksCanvas.enabled = true;
			}
			else
				barracksCanvas.enabled = false;
		}
    }

	public void GetChatMessage()
	{
		AddChatMessage(CurrentGameBoard.LocalGame.PlayerList[CurrentGameBoard.LocalPlayer].Name, chatDropdown.value);	
	}

    // Adds the passed string to the chat box. Used for both chat and system messages.
    public void AddChatMessage(string message)
    {
        chatBox.text += '\n' + message;
    }

    // Adds the passed string to the chat box. Used for both chat and system messages.
    public void AddChatMessage(string characterName, int messageIndex)
    {
		if(Constants.chatMessages[messageIndex] == Constants.chatMessages[Constants.playerBasedMessageIndex])
			chatBox.text += '\n' + characterName + Constants.chatMessages[messageIndex];
		else
			chatBox.text += '\n' + characterName + ": " + Constants.chatMessages[messageIndex];
    }

    // Sets the passed player number to the current winner of the longest road.
    public int SetLongestRoadWinner(int previousLongestRoad, int playerNumber)
    {
     // Code to set the passed player number to the current winner of the longest road.
		if(previousLongestRoad <= -1)
		{
			playerClassLocalArray[playerNumber].longestRoadIndicator = true;
			screenElementsArray[playerClassLocalArray[playerNumber].uiPosition].longestRoadIndicator.enabled = true;
            previousLongestRoad = playerNumber;
		}
		else
		{
			playerClassLocalArray[previousLongestRoad].longestRoadIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = false;
            playerClassLocalArray[playerNumber].longestRoadIndicator = true;
			screenElementsArray[playerClassLocalArray[playerNumber].uiPosition].longestRoadIndicator.enabled = true;
            previousLongestRoad = playerNumber;
		}

		return previousLongestRoad;
    }

    // Sets the passed player number to the current winner of the largest army.
    public void SetLargestArmyWinner(int previousLargestArmy, int playerNumber)
    {
		if(previousLargestArmy <= -1)
		{
			playerClassLocalArray[playerNumber].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[playerNumber].uiPosition].largestArmyIndicator.enabled = true;
		}
		else 
		{
			playerClassLocalArray[previousLargestArmy].largestArmyIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = false;
            playerClassLocalArray[playerNumber].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[playerNumber].uiPosition].largestArmyIndicator.enabled = true;
		}

    }

	public void NextPlayerLocal(int previousLongestRoad, int previousLargestArmy, bool reverse = false)
	{
        if (reverse == false)
        {
            if (playerNumberCurrent < BoardManager.numOfPlayers)
                playerNumberCurrent++;
            else
                playerNumberCurrent = 1;
            
            for (int count = 0; count < BoardManager.numOfPlayers; count++)
            {
                screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite>(playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].characterPictureName) as Sprite;
                screenElementsArray[count].playerColour.sprite = Resources.Load<Sprite>(playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].playerColourName) as Sprite;
                playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].uiPosition = count;
            }
            if (previousLongestRoad > -1)
            {
                if (playerClassLocalArray[previousLongestRoad].uiPosition < BoardManager.numOfPlayers - 1)
                {
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition + 1].longestRoadIndicator.enabled = false;
                }
                else
                {
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition - (BoardManager.numOfPlayers - 1)].longestRoadIndicator.enabled = false;
                }
            }

            if (previousLargestArmy > -1)
            {
                if (playerClassLocalArray[previousLargestArmy].uiPosition < BoardManager.numOfPlayers - 1)
                {
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition + 1].largestArmyIndicator.enabled = false;
                }
                else
                {
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition - (BoardManager.numOfPlayers - 1)].largestArmyIndicator.enabled = false;
                }
            }

            if (currentPlayer < BoardManager.numOfPlayers - 1)
                currentPlayer++;
            else
                currentPlayer = 0;
        }
        else
        {
            if(playerNumberCurrent > 0)
                playerNumberCurrent--;
            else
                playerNumberCurrent = BoardManager.numOfPlayers-1;

            for(int count = BoardManager.numOfPlayers-1; count >= 0; count--)
            {
                screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite> (playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].characterPictureName) as Sprite;
                screenElementsArray[count].playerColour.sprite = Resources.Load<Sprite>(playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].playerColourName) as Sprite;
                playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].uiPosition = count;
            }

            if(previousLongestRoad > -1)
            {
                if(playerClassLocalArray[previousLongestRoad].uiPosition >= 0)
                {
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition-1].longestRoadIndicator.enabled = false;
                }
                else
                {
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition + (BoardManager.numOfPlayers-1)].longestRoadIndicator.enabled = false;
                }
            }

            if(previousLargestArmy > -1)
            {
                if(playerClassLocalArray[previousLargestArmy].uiPosition >= 0)
                {
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition-1].largestArmyIndicator.enabled = false;
                }
                else
                {
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = true;
                    screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition + (BoardManager.numOfPlayers-1)].largestArmyIndicator.enabled = false;
                }
            }

            if(currentPlayer > 0)
                currentPlayer--;
            else
                currentPlayer = BoardManager.numOfPlayers-1;
        }

        UpdatePlayer();
	}

    public void NextPlayerNetwork(bool reverse = false)
    {
        int lastPlayer;

        if (reverse == false)
            lastPlayer = (CurrentGameBoard.CurrentPlayer == 0) ? (BoardManager.numOfPlayers - 1) : (CurrentGameBoard.CurrentPlayer - 1);
        else
            lastPlayer = CurrentGameBoard.CurrentPlayer + 1;

        screenElementsArray[playerClassLocalArray[lastPlayer].uiPosition].characterSelected.enabled = false;
        screenElementsArray[playerClassLocalArray[CurrentGameBoard.CurrentPlayer].uiPosition].characterSelected.enabled = true;
    }

	public void openShop()
	{
		if(shopCanvas.enabled == false)
			shopCanvas.enabled = true;
		else
			shopCanvas.enabled = false;
	}

	public void closeShop()
	{
		shopCanvas.enabled = false;
	}

	public void openBaracks()
	{
		if(barracksCanvas.enabled == false)
			barracksCanvas.enabled = true;
		else
			barracksCanvas.enabled = false;
	}

	public void closeBarracks()
	{
		barracksCanvas.enabled = false;
	}

    public void updateText()
    {
        chatBox.text += '\n' + chatInput.text;
    }

    public void goBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void resume()
    {
        escapeCanvas.enabled = false;
    }

    public void exitGame()
    {
        Application.Quit();
    }

	public void showOptions()
	{
		escapeCanvas.enabled = false;
		optionsCanvas.enabled = true;
    }

	public void hideOptions()
	{
		escapeCanvas.enabled = true;
		optionsCanvas.enabled = false;
	}

    public void DisableGameCanvas()
    {
        GameCanvasEnabled = false;
        BuildRoad.interactable = false;
        BuildSettlement.interactable = false;
        BuildCity.interactable = false;
        BuyArmy.interactable = false;
        Attack.interactable = false;
        Market.interactable = false;
        EndTurn.interactable = false;
    }

    public void EnableGameCanvas()
    {
        GameCanvasEnabled = true;
        BuildRoad.interactable = true;
        BuildSettlement.interactable = true;
        BuildCity.interactable = true;
        BuyArmy.interactable = true;
        Attack.interactable = true;
        Market.interactable = true;
        EndTurn.interactable = true;
    }

    public IEnumerator RollDice(int rollOne, int rollTwo)
    {
        DiceImage.enabled = true;
        DiceText.enabled = true;
        diceValue.text = (rollOne + rollTwo).ToString();
        yield return new WaitForSeconds(2);
        DiceImage.enabled = false;
        DiceText.enabled = false;
    }

    public void UpdatePlayer()
	{
        Player tempPlayer = CurrentGameBoard.LocalGame.PlayerList[CurrentGameBoard.LocalGame.isNetwork? CurrentGameBoard.LocalPlayer: CurrentGameBoard.CurrentPlayer];
        // Wheat
        wheatScore.text = tempPlayer.Wheat.ToString();
        // Brick
        brickScore.text = tempPlayer.Brick.ToString();
        // Wool
        sheepScore.text = tempPlayer.Wool.ToString();
        // Wood
        woodScore.text = tempPlayer.Wood.ToString();
        // Ore
        oreScore.text = tempPlayer.Ore.ToString();
        // Gold
        goldScore.text = tempPlayer.Gold.ToString();

        // Update the player portaits
        for (int x = 0; x < BoardManager.numOfPlayers; x++)
        {
            screenElementsArray[playerClassLocalArray[x].uiPosition].armySizeDisplay.text = CurrentGameBoard.LocalGame.PlayerList[x].Armies.ToString();
            screenElementsArray[playerClassLocalArray[x].uiPosition].longestRoadLengthDisplay.text = CurrentGameBoard.LocalGame.PlayerList[x].LongestRoad.ToString();
            screenElementsArray[playerClassLocalArray[x].uiPosition].victoryPoints.text = CurrentGameBoard.LocalGame.PlayerList[x].VictoryPoints.ToString();
        }

	}

	public void putLocalPlayerFirst(int networkPlayerNumberIndex)
	{
		if(networkPlayerNumberIndex > 0)
		{
			for(int count = 0; count < networkPlayerNumberIndex; count++)
			{
			    screenElementsArray[count+1].characterPicture.sprite = Resources.Load<Sprite>(playerClassLocalArray[count].characterPictureName) as Sprite;
				screenElementsArray[count+1].playerColour.sprite = Resources.Load<Sprite>(playerClassLocalArray[count].playerColourName) as Sprite;
				playerClassLocalArray[count].uiPosition = count+1;
			}

            screenElementsArray[0].characterPicture.sprite = Resources.Load<Sprite>(playerClassLocalArray[networkPlayerNumberIndex].characterPictureName) as Sprite;
            screenElementsArray[0].playerColour.sprite = Resources.Load<Sprite>(playerClassLocalArray[networkPlayerNumberIndex].playerColourName) as Sprite;
            playerClassLocalArray[networkPlayerNumberIndex].uiPosition = 0;
		}
	}

	public void showVictoryScreen() //*******************
	{
        for (int count = 0; count < BoardManager.numOfPlayers; count++)
        {
            scoreboardElementsArray[count].numOfArmies.text = CurrentGameBoard.LocalGame.PlayerList[count].Armies.ToString();
            scoreboardElementsArray[count].numOfConsecutiveRoads.text = CurrentGameBoard.LocalGame.PlayerList[count].LongestRoad.ToString();
            scoreboardElementsArray[count].numOfVP.text = CurrentGameBoard.LocalGame.PlayerList[count].VictoryPoints.ToString();
            scoreboardElementsArray[count].characterName.text = CurrentGameBoard.LocalGame.PlayerList[count].Name;
            scoreboardElementsArray[count].characterImage.sprite = Resources.Load<Sprite>(CurrentGameBoard.LocalGame.PlayerList[count].Name) as Sprite;

            if(CurrentGameBoard.LocalGame.PlayerList[count].LargestArmyWinner == true)
                scoreboardElementsArray[count].largestArmyIndicator.enabled = true;
            else
                scoreboardElementsArray[count].largestArmyIndicator.enabled = false;

            if(CurrentGameBoard.LocalGame.PlayerList[count].LongestRoadWinner == true)
                scoreboardElementsArray[count].longestRoadIndicator.enabled = true;
            else
                scoreboardElementsArray[count].longestRoadIndicator.enabled = false;

            if(CurrentGameBoard.LocalGame.PlayerList[count].GameWinner == true)
                scoreBoardWinnerText.text = CurrentGameBoard.LocalGame.PlayerList[count].Name + " has won the game!";
        }
        DisableGameCanvas();
        escapeCanvas.enabled = false;
        optionsCanvas.enabled = false;
        shopCanvas.enabled = false;
        barracksCanvas.enabled = false;
        DiceImage.enabled = false;
        DiceText.enabled = false;
        scoreboardCanvas.enabled = true;
	}

	public void setGameWinner(int playerNumber)
	{
		CurrentGameBoard.LocalGame.PlayerList[playerNumber].GameWinner = true;
	}

	public IEnumerator showPlayerTurnPopup(int playerNumber)
	{
		if(playerNumber == CurrentGameBoard.LocalPlayer)
		{
			inGamePopupText.text = "Your turn!";
		}
		else
		{
			inGamePopupText.text = CurrentGameBoard.LocalGame.PlayerList[playerNumber].Name + "'s turn!";
		}

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(2);
		inGamePopupText.enabled = false;
	}

	public IEnumerator showRobberMovedPopup(int playerNumber)
	{	
		if(playerNumber == CurrentGameBoard.LocalPlayer)
		{
			inGamePopupText.text = "You have moved the robber!";
		}
		else
		{
			inGamePopupText.text = CurrentGameBoard.LocalGame.PlayerList[playerNumber].Name + " has moved the robber!";
		}

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(Constants.popupWaitTime);
		inGamePopupText.enabled = false;
	}

	public IEnumerator showAttackedPopup(int playerNumber)
	{
		inGamePopupText.text = "Your city has been attacked by " + CurrentGameBoard.LocalGame.PlayerList[playerNumber].Name + "!";

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(Constants.popupWaitTime);
		inGamePopupText.enabled = false;
	}

	public IEnumerator showNotEnoughGoldPopup(int playerNumber)
	{
		inGamePopupText.text = "Your city has been attacked by " + CurrentGameBoard.LocalGame.PlayerList[playerNumber].Name + "!";

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(Constants.popupWaitTime);
		inGamePopupText.enabled = false;
	}

	public IEnumerator showNotEnoughResourcesToBuildPopup(int playerNumber)
	{
		inGamePopupText.text = "You do not have enough resources to build this!";

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(Constants.popupWaitTime);
		inGamePopupText.enabled = false;
	}

	public IEnumerator showNotEnoughResourcesToSellPopup(int playerNumber)
	{
		inGamePopupText.text = "You do not have enough resources to sell!";

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(Constants.popupWaitTime);
		inGamePopupText.enabled = false;
	}

	public IEnumerator showNoCitiesToPlaceArmiesIn(int playerNumber)
	{
		inGamePopupText.text = "You currently have no cities to place armies in";

		inGamePopupText.enabled = true;
		yield return new WaitForSeconds(Constants.popupWaitTime);
		inGamePopupText.enabled = false;
	}

	public void callAttackedPopup(int playerNumber)
	{
		StartCoroutine(showAttackedPopup(playerNumber));
	}

	public void callRobberMovedPopup(int playerNumber)
	{
		StartCoroutine(showRobberMovedPopup(playerNumber));
	}

	public void callPlayerTurnPopup(int playerNumber)
	{
		StartCoroutine(showPlayerTurnPopup(playerNumber));
	}

	public void callNotEnoughGoldPopup(int playerNumber)
	{
		StartCoroutine(showNotEnoughGoldPopup(playerNumber));
	}

	public void callNotEnoughResourcesToBuildPopup(int playerNumber)
	{
		StartCoroutine(showNotEnoughResourcesToBuildPopup(playerNumber));
	}

	public void callNotEnoughResourcesToSellPopup(int playerNumber)
	{
		StartCoroutine(showNotEnoughResourcesToSellPopup(playerNumber));
	}

	public void callNoCitiesToPlaceArmiesInPopup(int playerNumber)
	{
		StartCoroutine(showNoCitiesToPlaceArmiesIn(playerNumber));
	}
}