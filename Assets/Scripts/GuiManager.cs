using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {
    
    public const int MAX_NUM_OF_PLAYERS = 6;
	public const int LOCAL_PLAYER = 0;
    
    [System.Serializable]
    public class playerIndicatorClassLocal
    {
		public bool largestArmyIndicator = false;
		public bool longestRoadIndicator = false;
		public string characterPictureName;
        public int armySizeTotal;
        public int longestRoadLength;
        public int victoryPoints;
		public int uiPosition;
        public string playerName;
        public int armyQuantity   = 0,
				   brickQuantity  = 0,
	               goldQuantity   = 100,
				   oreQuantity    = 0,
	           	   sheepQuantity  = 0,
	           	   wheatQuantity  = 0,
	           	   woodQuantity   = 0,
				   numOfVictoryPoints = 0;
    }

    [System.Serializable]
	public class screenElements
	{
		public Canvas playerCanvas;
		public Image largestArmyIndicator;
		public Image longestRoadIndicator;
		public Image characterPicture;
		public Text armySizeDisplay;
		public Text longestRoadLengthDisplay;
		public Text victoryPoints;
		public Text playerName;
	}

    public InputField chatInput;
    public int actualNumOfPlayers;
    public Text brickScore;
    public Text chatBox;
    public Text diceValue;
    public Text goldScore;
    public Text oreScore;
    public Text sheepScore;
    public Text wheatScore;
    public Text woodScore;
    public Canvas gameCanvas,
   	              escapeCanvas,
		          optionsCanvas,
				  shopCanvas,
				  barracksCanvas;
           int armyBuyPrice   = 100,
			   armySellPrice  = 50,
			   brickBuyPrice  = 150,
			   brickSellPrice = 75,
			   oreBuyPrice    = 150,
			   oreSellPrice   = 75,
			   sheepBuyPrice  = 150,
			   sheepSellPrice = 75,
			   wheatBuyPrice  = 150,
			   wheatSellPrice = 75,
			   woodBuyPrice   = 150,
			   woodSellPrice  = 75,
			   currentPlayer  = 0,
 			   playerNumberCurrent = 1;

	public int // numOfPlayers,
    	       randomNumber1,
    	       randomNumber2,
    	       randomNumberActual;
           string player1Name = "GhostRag3: ";
    public System.Random randDiceObject = new System.Random();
    
    public playerIndicatorClassLocal[] playerClassLocalArray = new playerIndicatorClassLocal[MAX_NUM_OF_PLAYERS];
	public screenElements[] screenElementsArray = new screenElements[MAX_NUM_OF_PLAYERS];

    public GameBoard CurrentGameBoard;
    
    void Awake()
    {
        CurrentGameBoard = GameObject.Find("Map").GetComponent<GameBoard>();

        for (int count = 0; count < BoardManager.numOfPlayers; count++)
		{
			playerClassLocalArray[count].characterPictureName = characterSelect.selectedCharacters[count];
			playerClassLocalArray[count].uiPosition = count;
            screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite> (playerClassLocalArray[count].characterPictureName) as Sprite;
			screenElementsArray[count].largestArmyIndicator.enabled = false;
			screenElementsArray[count].longestRoadIndicator.enabled = false;
		}

	    gameCanvas.enabled    = true;
	    escapeCanvas.enabled  = false;
		optionsCanvas.enabled = false;
		shopCanvas.enabled    = false;
		barracksCanvas.enabled = false;
    }

	void Start()
	{
		for(int count = 0; count < MAX_NUM_OF_PLAYERS; count++)
			screenElementsArray[count].playerCanvas.enabled = false;
	}

    void Update ()
    {
        if (Input.GetKeyDown("escape"))
        {
            if(escapeCanvas.enabled == false)
                escapeCanvas.enabled = true;
            else
                escapeCanvas.enabled = false;
        }

	    if (Input.GetKeyDown("s"))
		{
			if(shopCanvas.enabled == false)
				shopCanvas.enabled = true;
			else
				shopCanvas.enabled = false;
		}

		if (Input.GetKeyDown("b"))
		{
			if(barracksCanvas.enabled == false)
				barracksCanvas.enabled = true;
			else
				barracksCanvas.enabled = false;
		}

		if(BoardManager.numOfPlayers >= 1)
		{
			screenElementsArray[0].playerCanvas.enabled = true;
			if(BoardManager.numOfPlayers >= 2)
			{		
				screenElementsArray[1].playerCanvas.enabled = true;
				if(BoardManager.numOfPlayers >= 3)
				{
					screenElementsArray[2].playerCanvas.enabled = true;
					if(BoardManager.numOfPlayers >=4)
					{
						screenElementsArray[3].playerCanvas.enabled = true;
						if(BoardManager.numOfPlayers >=5)
						{
							screenElementsArray[4].playerCanvas.enabled = true;
							if(BoardManager.numOfPlayers >=6)
								screenElementsArray[5].playerCanvas.enabled = true;
							else
								screenElementsArray[5].playerCanvas.enabled = false;
						}
						else
						{
							screenElementsArray[4].playerCanvas.enabled = false;
						}
					}
					else
					{
						screenElementsArray[3].playerCanvas.enabled = false;
					}
				}
				else
				{
					screenElementsArray[2].playerCanvas.enabled = false;
				}
			}
			else
			{
				screenElementsArray[1].playerCanvas.enabled = false;
			}
		}
		else
		{
			screenElementsArray[0].playerCanvas.enabled = false;
		}
    }

    // Sets the gold for the local player.
	public void SetPlayerGold(int goldAmount)
    {
        goldScore.text = goldAmount.ToString();
    }

	public void SetPlayerGold(int goldAmount, int playerNumber)
    {
		playerClassLocalArray[playerNumber].goldQuantity = goldAmount;
        goldScore.text = goldAmount.ToString();
    }

    // Sets the longest road length for the local player.
    public void SetLongestRoad(int largestNumOfConsecutiveRoads)
    {
        screenElementsArray[LOCAL_PLAYER].longestRoadLengthDisplay.text = largestNumOfConsecutiveRoads.ToString();
    }

    // Sets the longest road length shown for all other players, based on passed player number.
    public void SetLongestRoad(int largestNumOfConsecutiveRoads, int playerNumber) // Need number for previous player with longest road
    {
        screenElementsArray[playerNumber].longestRoadLengthDisplay.text = largestNumOfConsecutiveRoads.ToString();
    }

    // Sets the army size for the local player.
    public void SetArmySize(int armySize)
    {
        screenElementsArray[LOCAL_PLAYER].armySizeDisplay.text = armySize.ToString();
    }

    // Sets the army size shown for all other players, based on passed player number.
    public void SetArmySize(int armySize, int playerNumber)
    {
        screenElementsArray[playerNumber].armySizeDisplay.text = armySize.ToString();
    }

    // Sets the Victory Points shown for local player.
    public void SetPlayerVP(int victoryPointAmount)
    {
        screenElementsArray[LOCAL_PLAYER].victoryPoints.text = victoryPointAmount.ToString();
    }

    // Sets the Victory Points shown for all other players, based on passed player number.
    public void SetPlayerVP(int victoryPointAmount, int playerNumber)
    {
        screenElementsArray[playerNumber].victoryPoints.text = victoryPointAmount.ToString();
    }

    // Sets the dice to the passed dice roll.
    public void RollDice(int diceRoll)
    {
        // Code to animate the dice roll and show the rolled amount passed.
    }

    // Adds the passed string to the chat box. Used for both chat and system messages.
    public void AddChatMessage(int playerNumber, string message)
    {
        chatBox.text += '\n' + playerClassLocalArray[playerNumber].playerName + ": " + message;
    }

    // Sets the local player to the current winner of the longest road.
	public int SetLongestRoadWinner(int previousLongestRoad)
    {
     // Code to the local player to the current winner of the longest road.
		if(previousLongestRoad <= -1)
		{
			playerClassLocalArray[LOCAL_PLAYER].longestRoadIndicator = true;
			screenElementsArray[playerClassLocalArray[LOCAL_PLAYER].uiPosition].longestRoadIndicator.enabled = true;
			previousLongestRoad = LOCAL_PLAYER;
		}
		else
		{
			playerClassLocalArray[previousLongestRoad].longestRoadIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = false;
			playerClassLocalArray[LOCAL_PLAYER].longestRoadIndicator = true;
			screenElementsArray[playerClassLocalArray[LOCAL_PLAYER].uiPosition].longestRoadIndicator.enabled = true;
			previousLongestRoad = LOCAL_PLAYER;
		}

		return previousLongestRoad;
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

    // Sets the local player to the current winner of the largest army.
    public int SetLargestArmyWinner(int previousLargestArmy)
    {
		if(previousLargestArmy <= -1)
		{
			playerClassLocalArray[LOCAL_PLAYER].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[LOCAL_PLAYER].uiPosition].largestArmyIndicator.enabled = true;
			previousLargestArmy = LOCAL_PLAYER;
		}
		else 
		{
			playerClassLocalArray[previousLargestArmy].largestArmyIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = false;
			playerClassLocalArray[LOCAL_PLAYER].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[LOCAL_PLAYER].uiPosition].largestArmyIndicator.enabled = true;
			previousLargestArmy = LOCAL_PLAYER;
		}

		return previousLargestArmy;
    }

    // Sets the passed player number to the current winner of the largest army.
    public int SetLargestArmyWinner(int previousLargestArmy, int playerNumber)
    {
		if(previousLargestArmy <= -1)
		{
			playerClassLocalArray[playerNumber].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[playerNumber].uiPosition].largestArmyIndicator.enabled = true;
			previousLargestArmy = playerNumber;
		}
		else 
		{
			playerClassLocalArray[previousLargestArmy].largestArmyIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = false;
			playerClassLocalArray[playerNumber].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[playerNumber].uiPosition].largestArmyIndicator.enabled = true;
			previousLargestArmy = playerNumber;
		}

		return previousLargestArmy;
    }

    // Updates the timer display with the passed amount of time remaining.
    public void UpdateTimer(int timeLeft)
    {
     // Code tp update the timer. For now just create a text box that will display the time in seconds remaining.
    }

	public void swapPlayer(int previousLongestRoad, int previousLargestArmy)
	{
		for(int count = 0; count < BoardManager.numOfPlayers; count++)
		{
	        screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite> (playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].characterPictureName) as Sprite;
			playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].uiPosition = count;
		}
		if(previousLongestRoad > -1)
		{
			if(playerClassLocalArray[previousLongestRoad].uiPosition < BoardManager.numOfPlayers-1)
			{
				screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition+1].longestRoadIndicator.enabled = false;
			}
			else
			{
				screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition].longestRoadIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLongestRoad].uiPosition - (BoardManager.numOfPlayers-1)].longestRoadIndicator.enabled = false;
			}
		}

		if(previousLargestArmy > -1)
		{
			if(playerClassLocalArray[previousLargestArmy].uiPosition < BoardManager.numOfPlayers-1)
			{
				screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition+1].largestArmyIndicator.enabled = false;
			}
			else
			{
				screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition].largestArmyIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLargestArmy].uiPosition - (BoardManager.numOfPlayers-1)].largestArmyIndicator.enabled = false;
			}
		}

		if(playerNumberCurrent < BoardManager.numOfPlayers)
			playerNumberCurrent++;
		else
			playerNumberCurrent = 1;

		if(currentPlayer < BoardManager.numOfPlayers-1)
			currentPlayer++;
		else
			currentPlayer = 0;

		brickScore.text = playerClassLocalArray[currentPlayer].brickQuantity.ToString();
		oreScore.text = playerClassLocalArray[currentPlayer].oreQuantity.ToString();
		wheatScore.text = playerClassLocalArray[currentPlayer].wheatQuantity.ToString();
		woodScore.text = playerClassLocalArray[currentPlayer].woodQuantity.ToString();
		sheepScore.text = playerClassLocalArray[currentPlayer].sheepQuantity.ToString();
	}

	public void moreArmy(int playerNumber)
	{
		playerClassLocalArray[playerNumber].armyQuantity++;
	}

	public void lessArmy(int playerNumber)
	{
		playerClassLocalArray[playerNumber].armyQuantity--;
	}

    public void moreWheat(int playerNumber)
    {
        playerClassLocalArray[playerNumber].wheatQuantity++;
        wheatScore.text = playerClassLocalArray[playerNumber].wheatQuantity.ToString(); 
    }

    public void lessWheat(int playerNumber)
    {
        if(playerClassLocalArray[playerNumber].wheatQuantity > 0)
        {
            playerClassLocalArray[playerNumber].wheatQuantity--;
            wheatScore.text = playerClassLocalArray[playerNumber].wheatQuantity.ToString();
        }
    }
       
    public void moreSheep(int playerNumber)
    {
        playerClassLocalArray[playerNumber].sheepQuantity++;
        sheepScore.text = playerClassLocalArray[playerNumber].sheepQuantity.ToString();
    }

    public void lessSheep(int playerNumber)
    {
        if(playerClassLocalArray[playerNumber].sheepQuantity > 0)
        {
            playerClassLocalArray[playerNumber].sheepQuantity--;
            sheepScore.text = playerClassLocalArray[playerNumber].sheepQuantity.ToString();
        }
    }
       
    public void moreBrick(int playerNumber)
    {
        playerClassLocalArray[playerNumber].brickQuantity++;
        brickScore.text = playerClassLocalArray[playerNumber].brickQuantity.ToString();
    } 

    public void lessBrick(int playerNumber)
    {
        if(playerClassLocalArray[playerNumber].brickQuantity > 0)
        {
            playerClassLocalArray[playerNumber].brickQuantity--;
            brickScore.text = playerClassLocalArray[playerNumber].brickQuantity.ToString();
        }
    }   
    public void moreWood(int playerNumber)
    {
        playerClassLocalArray[playerNumber].woodQuantity++;
        woodScore.text = playerClassLocalArray[playerNumber].woodQuantity.ToString();
    }

    public void lessWood(int playerNumber)
    {
        if(playerClassLocalArray[playerNumber].woodQuantity > 0)
        {
            playerClassLocalArray[playerNumber].woodQuantity--;
            woodScore.text = playerClassLocalArray[playerNumber].woodQuantity.ToString();
        }
    }

    public void moreOre(int playerNumber)
    {
        playerClassLocalArray[playerNumber].oreQuantity++;
        oreScore.text = playerClassLocalArray[playerNumber].oreQuantity.ToString();
    }

	public void rollDice(int rollOne, int rollTwo)
    {
        diceValue.text = (rollOne + rollTwo).ToString();
    }

    public void lessOre(int playerNumber)
    {
        if(playerClassLocalArray[playerNumber].oreQuantity > 0)
        {
            playerClassLocalArray[playerNumber].oreQuantity--;  
            oreScore.text = playerClassLocalArray[playerNumber].oreQuantity.ToString();
        }
    }

    public void moreGoldSettlement(int playerNumber)
    {
        playerClassLocalArray[playerNumber].goldQuantity += 20;
        goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
    }

    public void moreGoldCity(int playerNumber)
    {
        playerClassLocalArray[playerNumber].goldQuantity += 50;
        goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
    }

    public void lessGold(int playerNumber)
    {
        if(playerClassLocalArray[playerNumber].goldQuantity > 0)
        {
            playerClassLocalArray[playerNumber].goldQuantity--;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
        }
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

	public void buyArmy(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].goldQuantity >= armyBuyPrice)
		{
			moreArmy(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity -= armyBuyPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellArmy(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].armyQuantity > 0)
		{
			lessArmy(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity += armySellPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void buyWood(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].goldQuantity >= woodBuyPrice)
		{
			moreWood(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity -= woodBuyPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellWood(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].woodQuantity > 0)
		{
			lessWood(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity += woodSellPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough wood
		}
	}

	public void buySheep(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].goldQuantity >= sheepBuyPrice)
		{
			moreSheep(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity -= sheepBuyPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellSheep(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].sheepQuantity > 0)
		{
			lessSheep(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity += sheepSellPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough sheep
		}
	}

	public void buyBrick(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].goldQuantity >= brickBuyPrice)
		{
			moreBrick(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity -= brickBuyPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellBrick(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].brickQuantity > 0)
		{
			lessBrick(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity += brickSellPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough brick
		}
	}

	public void buyOre(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].goldQuantity >= oreBuyPrice)
		{
			moreOre(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity -= oreBuyPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellOre(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].oreQuantity > 0)
		{
			lessOre(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity += oreSellPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough ore
		}
	}

	public void buyWheat(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].goldQuantity >= wheatBuyPrice)
		{
			moreWheat(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity -= wheatBuyPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellWheat(int playerNumber)
	{
		if(playerClassLocalArray[playerNumber].wheatQuantity > 0)
		{
			lessWheat(playerNumber);
			playerClassLocalArray[playerNumber].goldQuantity += wheatSellPrice;
            goldScore.text = playerClassLocalArray[playerNumber].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough wheat
		}
	}

    public void updateText()
    {
        chatBox.text += '\n' + player1Name + chatInput.text;
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

	public void UpdatePlayer()
	{
        Player tempPlayer = CurrentGameBoard.LocalGame.PlayerList[CurrentGameBoard.CurrentPlayer];
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
        // VP
    }
}
