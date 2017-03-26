using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class gameUIScriptLocal : MonoBehaviour {

    [System.Serializable]
    public class playerIndicatorClassLocal //*************************
    {
		public bool largestArmyIndicator = false;
		public bool longestRoadIndicator = false;
		public string characterPictureName;
        public int armySizeTotal;
        public int longestRoadLength;
        public int victoryPoints;
		public int uiPosition;
        public string playerName;
        public int armyQuantity   = 0,// *********
				   brickQuantity  = 0,
	               goldQuantity   = 100,
				   oreQuantity    = 0,
	           	   sheepQuantity  = 0,
	           	   wheatQuantity  = 0,
	           	   woodQuantity   = 0,
				   numOfVictoryPoints = 0;// *********
    }

	[System.Serializable]
	public class screenElements //*******************************
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

	public const int MAX_PLAYERS = 6;
    public Canvas gameCanvas,
   	              escapeCanvas,
		          optionsCanvas,
				  shopCanvas;
    public InputField chatInput;
           int armyBuyPrice   = 100,// *********
			   armySellPrice  = 50,// *********
			   brickBuyPrice  = 150,// *********
			   brickSellPrice = 75,// *********
			   oreBuyPrice    = 150,// *********
			   oreSellPrice   = 75,// *********
			   sheepBuyPrice  = 150,// *********
			   sheepSellPrice = 75,// *********
			   wheatBuyPrice  = 150,// *********
			   wheatSellPrice = 75,// *********
			   woodBuyPrice   = 150,// *********
			   woodSellPrice  = 75,// *********
			   currentPlayer  = 0,// *********
			   previousLongestRoadWinner = -1,// *********
			   previousLargestArmyWinner = -1,// *********
 			   playerNumberCurrent = 1;// *********

	public int // numOfPlayers,
    	       randomNumber1,
    	       randomNumber2,
    	       randomNumberActual;
           string player1Name = "GhostRag3: ";
    public System.Random randDiceObject = new System.Random();
    public Text brickScore,
    		    chatBox,
    		    diceValue,
                goldScore,
    	        oreScore,
    		    sheepScore,
    	        wheatScore,
    		    woodScore;
	public playerIndicatorClassLocal[] playerClassLocalArray = new playerIndicatorClassLocal[MAX_PLAYERS];//*****************************
	public screenElements[] screenElementsArray = new screenElements[MAX_PLAYERS]; //*****************************
	// public string[] imageNames = new string[MAX_PLAYERS];

    void Awake()
    {
		gameCanvas.enabled    = true;
	    escapeCanvas.enabled  = false;
		optionsCanvas.enabled = false;
		shopCanvas.enabled    = false;//********** 
		
        for(int count = 0; count < BoardManager.numOfPlayers; count++)
		{
			playerClassLocalArray[count].characterPictureName = characterSelect.selectedCharacters[count];//*****************************
			playerClassLocalArray[count].uiPosition = count;
            screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite> (playerClassLocalArray[count].characterPictureName) as Sprite;//*****************************
			screenElementsArray[count].largestArmyIndicator.enabled = false;
			screenElementsArray[count].longestRoadIndicator.enabled = false;
		}

	    
    }

	void Start()
	{
		for(int count = 0; count < MAX_PLAYERS; count++)
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

	    if (Input.GetKeyDown("s"))//**********
		{
			openShop();
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

	public void moreArmy()//*****************************
	{
		playerClassLocalArray[currentPlayer].armyQuantity++;
	}

	public void lessArmy()//*****************************
	{
		playerClassLocalArray[currentPlayer].armyQuantity--;
	}

    public void moreWheat()//*****************************
    {
        playerClassLocalArray[currentPlayer].wheatQuantity++;
        wheatScore.text = playerClassLocalArray[currentPlayer].wheatQuantity.ToString(); 
    }

    public void lessWheat()//*****************************
    {
        if(playerClassLocalArray[currentPlayer].wheatQuantity > 0)
        {
            playerClassLocalArray[currentPlayer].wheatQuantity--;
            wheatScore.text = playerClassLocalArray[currentPlayer].wheatQuantity.ToString();
        }
    }
       
    public void moreSheep()//*****************************
    {
        playerClassLocalArray[currentPlayer].sheepQuantity++;
        sheepScore.text = playerClassLocalArray[currentPlayer].sheepQuantity.ToString();
    }

    public void lessSheep()//*****************************
    {
        if(playerClassLocalArray[currentPlayer].sheepQuantity > 0)
        {
            playerClassLocalArray[currentPlayer].sheepQuantity--;
            sheepScore.text = playerClassLocalArray[currentPlayer].sheepQuantity.ToString();
        }
    }
       
    public void moreBrick()//*****************************
    {
        playerClassLocalArray[currentPlayer].brickQuantity++;
        brickScore.text = playerClassLocalArray[currentPlayer].brickQuantity.ToString();
    } 

    public void lessBrick()//*****************************
    {
        if(playerClassLocalArray[currentPlayer].brickQuantity > 0)
        {
            playerClassLocalArray[currentPlayer].brickQuantity--;
            brickScore.text = playerClassLocalArray[currentPlayer].brickQuantity.ToString();
        }
    }   
    public void moreWood()//*****************************
    {
        playerClassLocalArray[currentPlayer].woodQuantity++;
        woodScore.text = playerClassLocalArray[currentPlayer].woodQuantity.ToString();
    }

    public void lessWood()//*****************************
    {
        if(playerClassLocalArray[currentPlayer].woodQuantity > 0)
        {
            playerClassLocalArray[currentPlayer].woodQuantity--;
            woodScore.text = playerClassLocalArray[currentPlayer].woodQuantity.ToString();
        }
    }

    public void moreOre()//*****************************
    {
        playerClassLocalArray[currentPlayer].oreQuantity++;
        oreScore.text = playerClassLocalArray[currentPlayer].oreQuantity.ToString();
    }

    public void randDice()
    {
        randomNumber1 = randDiceObject.Next(1, 7);
        randomNumber2 = randDiceObject.Next(1, 7);
        randomNumberActual = randomNumber1 + randomNumber2;

        if(randomNumberActual == 4 || randomNumberActual == 8 || randomNumberActual == 12 || randomNumberActual == 6 || randomNumberActual == 10 || randomNumberActual == 11)
        {
            moreSheep();
            moreBrick();
        }
        else if(randomNumberActual == 3 || randomNumberActual == 2 || randomNumberActual == 5 || randomNumberActual == 7 || randomNumberActual == 9)
        {
            moreOre();
            moreWood();
            moreWheat();
        }             
        diceValue.text = randomNumberActual.ToString();
    }

    public void lessOre()//*****************************
    {
        if(playerClassLocalArray[currentPlayer].oreQuantity > 0)
        {
            playerClassLocalArray[currentPlayer].oreQuantity--;  
            oreScore.text = playerClassLocalArray[currentPlayer].oreQuantity.ToString();
        }
    }

    public void moreGoldSettlement()//*****************************
    {
        playerClassLocalArray[currentPlayer].goldQuantity += 20;//*****************************
        goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();//*****************************
    }

    public void moreGoldCity()//*****************************
    {
        playerClassLocalArray[currentPlayer].goldQuantity += 50;//*****************************
        goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();//*****************************
    }

    public void lessGold()//*****************************
    {
        if(playerClassLocalArray[currentPlayer].goldQuantity > 0)
        {
            playerClassLocalArray[currentPlayer].goldQuantity--;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
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

	public void addPlayers()
	{
        if(BoardManager.numOfPlayers < MAX_PLAYERS)
            BoardManager.numOfPlayers++;
				
	}

	public void removePlayers()
	{
		if(BoardManager.numOfPlayers > 0)
			BoardManager.numOfPlayers--;
	}

	public void swapPlayer()
	{   
		for(int count = 0; count < BoardManager.numOfPlayers; count++)
		{
	        screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite> (playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].characterPictureName) as Sprite;
			screenElementsArray[count].playerName.text = playerClassLocalArray[(playerNumberCurrent+count) % BoardManager.numOfPlayers].playerName;
			playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].uiPosition = count;
		}//*******************************************
		if(previousLongestRoadWinner > -1)
		{
			if(playerClassLocalArray[previousLongestRoadWinner].uiPosition < BoardManager.numOfPlayers-1)
			{
				screenElementsArray[playerClassLocalArray[previousLongestRoadWinner].uiPosition].longestRoadIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLongestRoadWinner].uiPosition+1].longestRoadIndicator.enabled = false;
			}
			else
			{
				screenElementsArray[playerClassLocalArray[previousLongestRoadWinner].uiPosition].longestRoadIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLongestRoadWinner].uiPosition - (BoardManager.numOfPlayers-1)].longestRoadIndicator.enabled = false;
			}
		}

		if(previousLargestArmyWinner > -1)
		{
			if(playerClassLocalArray[previousLargestArmyWinner].uiPosition < BoardManager.numOfPlayers-1)
			{
				screenElementsArray[playerClassLocalArray[previousLargestArmyWinner].uiPosition].largestArmyIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLargestArmyWinner].uiPosition+1].largestArmyIndicator.enabled = false;
			}
			else
			{
				screenElementsArray[playerClassLocalArray[previousLargestArmyWinner].uiPosition].largestArmyIndicator.enabled = true;
				screenElementsArray[playerClassLocalArray[previousLargestArmyWinner].uiPosition - (BoardManager.numOfPlayers-1)].largestArmyIndicator.enabled = false;
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
			Debug.Log("numofPlayercurrrent" + playerNumberCurrent);	
	}

	public void openShop()// *********
	{
		if(shopCanvas.enabled == false)
			shopCanvas.enabled = true;
		else
			shopCanvas.enabled = false;
	}

	public void closeShop()// *************
	{
		shopCanvas.enabled = false;
	}

	public void buyArmy()// *********
	{
		if(playerClassLocalArray[currentPlayer].goldQuantity >= armyBuyPrice)
		{
			moreArmy();
			playerClassLocalArray[currentPlayer].goldQuantity -= armyBuyPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellArmy()// *********
	{
		if(playerClassLocalArray[currentPlayer].armyQuantity > 0)
		{
			lessArmy();
			playerClassLocalArray[currentPlayer].goldQuantity += armySellPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void buyWood()// *********
	{
		if(playerClassLocalArray[currentPlayer].goldQuantity >= woodBuyPrice)
		{
			moreWood();
			playerClassLocalArray[currentPlayer].goldQuantity -= woodBuyPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellWood()// *********
	{
		if(playerClassLocalArray[currentPlayer].woodQuantity > 0)
		{
			lessWood();
			playerClassLocalArray[currentPlayer].goldQuantity += woodSellPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough wood
		}
	}

	public void buySheep()// *********
	{
		if(playerClassLocalArray[currentPlayer].goldQuantity >= sheepBuyPrice)
		{
			moreSheep();
			playerClassLocalArray[currentPlayer].goldQuantity -= sheepBuyPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellSheep()// *********
	{
		if(playerClassLocalArray[currentPlayer].sheepQuantity > 0)
		{
			lessSheep();
			playerClassLocalArray[currentPlayer].goldQuantity += sheepSellPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough sheep
		}
	}

	public void buyBrick()// *********
	{
		if(playerClassLocalArray[currentPlayer].goldQuantity >= brickBuyPrice)
		{
			moreBrick();
			playerClassLocalArray[currentPlayer].goldQuantity -= brickBuyPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellBrick()// *********
	{
		if(playerClassLocalArray[currentPlayer].brickQuantity > 0)
		{
			lessBrick();
			playerClassLocalArray[currentPlayer].goldQuantity += brickSellPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough brick
		}
	}

	public void buyOre()// *********
	{
		if(playerClassLocalArray[currentPlayer].goldQuantity >= oreBuyPrice)
		{
			moreOre();
			playerClassLocalArray[currentPlayer].goldQuantity -= oreBuyPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellOre()// *********
	{
		if(playerClassLocalArray[currentPlayer].oreQuantity > 0)
		{
			lessOre();
			playerClassLocalArray[currentPlayer].goldQuantity += oreSellPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough ore
		}
	}

	public void buyWheat()// *********
	{
		if(playerClassLocalArray[currentPlayer].goldQuantity >= wheatBuyPrice)
		{
			moreWheat();
			playerClassLocalArray[currentPlayer].goldQuantity -= wheatBuyPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellWheat()// *********
	{
		if(playerClassLocalArray[currentPlayer].wheatQuantity > 0)
		{
			lessWheat();
			playerClassLocalArray[currentPlayer].goldQuantity += wheatSellPrice;
            goldScore.text = playerClassLocalArray[currentPlayer].goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough wheat
		}
	}

	public void setLongestRoadWinner()// *********
	{
		if(previousLongestRoadWinner <= -1)
		{
			playerClassLocalArray[currentPlayer].longestRoadIndicator = true;
			screenElementsArray[playerClassLocalArray[currentPlayer].uiPosition].longestRoadIndicator.enabled = true;
			previousLongestRoadWinner = currentPlayer;
		}
		else
		{
			playerClassLocalArray[previousLongestRoadWinner].longestRoadIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLongestRoadWinner].uiPosition].longestRoadIndicator.enabled = false;
			playerClassLocalArray[currentPlayer].longestRoadIndicator = true;
			screenElementsArray[playerClassLocalArray[currentPlayer].uiPosition].longestRoadIndicator.enabled = true;
			previousLongestRoadWinner = currentPlayer;
		}
	}

	public void setLargestArmyWinner()// *********
	{
		if(previousLargestArmyWinner <= -1)
		{
			playerClassLocalArray[currentPlayer].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[currentPlayer].uiPosition].largestArmyIndicator.enabled = true;
			previousLargestArmyWinner = currentPlayer;
		}
		else 
		{
			playerClassLocalArray[previousLargestArmyWinner].largestArmyIndicator = false;
			screenElementsArray[playerClassLocalArray[previousLargestArmyWinner].uiPosition].largestArmyIndicator.enabled = false;
			playerClassLocalArray[currentPlayer].largestArmyIndicator = true;
			screenElementsArray[playerClassLocalArray[currentPlayer].uiPosition].largestArmyIndicator.enabled = true;
			previousLargestArmyWinner = currentPlayer;
		}
	}
}