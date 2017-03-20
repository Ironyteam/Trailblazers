using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class gameUIScriptNetwork : MonoBehaviour {

    [System.Serializable]
    public class playerIndicatorClassNetwork
    {
        public bool largestArmyIndicator;
        public bool longestRoadIndicator;
		public string characterPictureName;
        public int armySizeTotal;
        public int longestRoadLength;
        public int victoryPoints;
        public string playerName;
    }

    [System.Serializable]
    public class playerIndicatorClassLocal
    {
		public bool largestArmyIndicator;
		public bool longestRoadIndicator;
		public string characterPictureName;
        public int armySizeTotal;
        public int longestRoadLength;
        public int victoryPoints;
        public string playerName;
        public int armyQuantity   = 0,// *********
				   brickQuantity  = 0,
	               goldQuantity   = 100,
				   oreSellPrice   = 75,// *********
	           	   sheepQuantity  = 0,
	           	   wheatQuantity  = 0,
	           	   woodQuantity   = 0,
				   numOfVictoryPoints = 0;// *********
    }

	[System.Serializable]
	public class screenElements
	{
		public Image largestArmyIndicator;
		public Image longestRoadIndicator;
		public Image characterPicture;
		public Text victoryPoints;
		public string playerName;
	}

	public const int MAX_PLAYERS = 6;
    public Canvas gameCanvas,
   	              escapeCanvas,
		          optionsCanvas,
				  player1Canvas,
				  player2Canvas,
				  player3Canvas,
				  player4Canvas,
				  player5Canvas,
				  player6Canvas,
				  shopCanvas;
    public InputField chatInput;
           int armyBuyPrice   = 100,// *********
			   armyQuantity   = 0,// *********
			   armySellPrice  = 50,// *********
			   brickQuantity  = 0,
			   brickBuyPrice  = 150,// *********
			   brickSellPrice = 75,// *********
               goldQuantity   = 100,
               oreQuantity    = 0,
			   oreBuyPrice    = 150,// *********
			   oreSellPrice   = 75,// *********
           	   sheepQuantity  = 0,
			   sheepBuyPrice  = 150,// *********
			   sheepSellPrice = 75,// *********
           	   wheatQuantity  = 0,
			   wheatBuyPrice  = 150,// *********
			   wheatSellPrice = 75,// *********
           	   woodQuantity   = 0,
			   woodBuyPrice   = 150,// *********
			   woodSellPrice  = 75;// *********
	public int // numOfPlayers,
    	       randomNumber1,
    	       randomNumber2,
    	       randomNumberActual;
           string player1Name = "GhostRag3: ";
    public System.Random randDiceObject = new System.Random();
	public Button attackButton,
				  marketButton,
				  buildSettlementButton,
			      buildRoadButton,
				  upgradeToCityButton;				  
    public Text brickScore,
    		    chatBox,
    		    diceValue,
                goldScore,
    	        oreScore,
    		    sheepScore,
    	        wheatScore,
    		    woodScore;
    public Image[] characterImages = new Image[MAX_PLAYERS];
	public playerIndicatorClassLocal[] playerClassLocalArray = new playerIndicatorClassLocal[MAX_PLAYERS];
	public screenElements[] screenElementsArray = new screenElements[MAX_PLAYERS];
	// public string[] imageNames = new string[MAX_PLAYERS];
	public int imageNumberCurrent = 1;

    void Awake()
    {
        for(int count = 0; count < BoardManager.numOfPlayers; count++)
            characterImages[count].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[count]) as Sprite;

	    gameCanvas.enabled    = true;
	    escapeCanvas.enabled  = false;
		optionsCanvas.enabled = false;
		shopCanvas.enabled    = false;//**********
    }

	void Start()
	{
		player1Canvas.enabled = false;
		player2Canvas.enabled = false;
		player3Canvas.enabled = false;
		player4Canvas.enabled = false;
		player5Canvas.enabled = false;
		player6Canvas.enabled = false;
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
			player1Canvas.enabled = true;
			if(BoardManager.numOfPlayers >= 2)
			{		
				player2Canvas.enabled = true;
				if(BoardManager.numOfPlayers >= 3)
				{
					player3Canvas.enabled = true;
					if(BoardManager.numOfPlayers >=4)
					{
						player4Canvas.enabled = true;
						if(BoardManager.numOfPlayers >=5)
						{
							player5Canvas.enabled = true;
							if(BoardManager.numOfPlayers >=6)
								player6Canvas.enabled = true;
							else
								player6Canvas.enabled = false;
						}
						else
						{
							player5Canvas.enabled = false;
						}
					}
					else
					{
						player4Canvas.enabled = false;
					}
				}
				else
				{
					player3Canvas.enabled = false;
				}
			}
			else
			{
				player2Canvas.enabled = false;
			}
		}
		else
		{
			player1Canvas.enabled = false;
		}

    }

	public void moreArmy()
	{
		armyQuantity++;
	}

	public void lessArmy()
	{
		armyQuantity--;
	}

    public void moreWheat()
    {
        wheatQuantity++;
        wheatScore.text = wheatQuantity.ToString(); 
    }

    public void lessWheat()
    {
        if(wheatQuantity > 0)
        {
            wheatQuantity--;
            wheatScore.text = wheatQuantity.ToString();
        }
    }
       
    public void moreSheep()
    {
        sheepQuantity++;
        sheepScore.text = sheepQuantity.ToString();
    }

    public void lessSheep()
    {
        if(sheepQuantity > 0)
        {
            sheepQuantity--;
            sheepScore.text = sheepQuantity.ToString();
        }
    }
       
    public void moreBrick()
    {
        brickQuantity++;
        brickScore.text = brickQuantity.ToString();
    } 

    public void lessBrick()
    {
        if(brickQuantity > 0)
        {
            brickQuantity--;
            brickScore.text = brickQuantity.ToString();
        }
    }   
    public void moreWood()
    {
        woodQuantity++;
        woodScore.text = woodQuantity.ToString();
    }

    public void lessWood()
    {
        if(woodQuantity > 0)
        {
            woodQuantity--;
            woodScore.text = woodQuantity.ToString();
        }
    }

    public void moreOre()
    {
        oreQuantity++;
        oreScore.text = oreQuantity.ToString();
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

    public void lessOre()
    {
        if(oreQuantity > 0)
        {
            oreQuantity--;  
            oreScore.text = oreQuantity.ToString();
        }
    }

    public void moreGoldSettlement()
    {
        goldQuantity += 20;
        goldScore.text = goldQuantity.ToString();
    }

    public void moreGoldCity()
    {
        goldQuantity += 50;
        goldScore.text = goldQuantity.ToString();
    }

    public void lessGold()
    {
        if(goldQuantity > 0)
        {
            goldQuantity--;
            goldScore.text = goldQuantity.ToString();
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
    /*    characterImage1.sprite = Resources.Load<Sprite> (imageNames[imageNumberCurrent % 6]) as Sprite;
        Debug.Log(imageNumberCurrent % 6);
        characterImage2.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 1) % 6]) as Sprite;
        Debug.Log((imageNumberCurrent + 1) % 6);

        characterImage3.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 2) % 6]) as Sprite;
        Debug.Log((imageNumberCurrent + 2) % 6);

		characterImage4.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 3) % 6]) as Sprite;
		Debug.Log((imageNumberCurrent + 3) % 6);

        characterImage5.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 4) % 6]) as Sprite;
        Debug.Log((imageNumberCurrent + 4) % 6);

        characterImage6.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 5) % 6]) as Sprite;
        Debug.Log((imageNumberCurrent + 5) % 6); */
        
        characterImages[0].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[imageNumberCurrent % BoardManager.numOfPlayers]) as Sprite;
        Debug.Log(imageNumberCurrent % BoardManager.numOfPlayers);
        
        characterImages[1].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[(imageNumberCurrent + 1) % BoardManager.numOfPlayers]) as Sprite;
        Debug.Log((imageNumberCurrent + 1) % BoardManager.numOfPlayers);

        characterImages[2].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[(imageNumberCurrent + 2) % BoardManager.numOfPlayers]) as Sprite;
        Debug.Log((imageNumberCurrent + 2) % BoardManager.numOfPlayers);

		characterImages[3].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[(imageNumberCurrent + 3) % BoardManager.numOfPlayers]) as Sprite;
		Debug.Log((imageNumberCurrent + 3) % BoardManager.numOfPlayers);

        characterImages[4].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[(imageNumberCurrent + 4) % BoardManager.numOfPlayers]) as Sprite;
        Debug.Log((imageNumberCurrent + 4) % BoardManager.numOfPlayers);

        characterImages[5].sprite = Resources.Load<Sprite> (characterSelect.selectedCharacters[(imageNumberCurrent + 5) % BoardManager.numOfPlayers]) as Sprite;
        Debug.Log((imageNumberCurrent + 5) % BoardManager.numOfPlayers);
                
		imageNumberCurrent++;

		if(imageNumberCurrent > BoardManager.numOfPlayers)
		{
			imageNumberCurrent = 1;
		}
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
		if(goldQuantity >= armyBuyPrice)
		{
			moreArmy();
			goldQuantity -= armyBuyPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellArmy()// *********
	{
		if(armyQuantity > 0)
		{
			lessArmy();
			goldQuantity += armySellPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void buyWood()// *********
	{
		if(goldQuantity >= woodBuyPrice)
		{
			moreWood();
			goldQuantity -= woodBuyPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellWood()// *********
	{
		if(woodQuantity > 0)
		{
			lessWood();
			goldQuantity += woodSellPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough wood
		}
	}

	public void buySheep()// *********
	{
		if(goldQuantity >= sheepBuyPrice)
		{
			moreSheep();
			goldQuantity -= sheepBuyPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellSheep()// *********
	{
		if(sheepQuantity > 0)
		{
			lessSheep();
			goldQuantity += sheepSellPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough sheep
		}
	}

	public void buyBrick()// *********
	{
		if(goldQuantity >= brickBuyPrice)
		{
			moreBrick();
			goldQuantity -= brickBuyPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellBrick()// *********
	{
		if(brickQuantity > 0)
		{
			lessBrick();
			goldQuantity += brickSellPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough brick
		}
	}

	public void buyOre()// *********
	{
		if(goldQuantity >= oreBuyPrice)
		{
			moreOre();
			goldQuantity -= oreBuyPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellOre()// *********
	{
		if(oreQuantity > 0)
		{
			lessOre();
			goldQuantity += oreSellPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough ore
		}
	}

	public void buyWheat()// *********
	{
		if(goldQuantity >= wheatBuyPrice)
		{
			moreWheat();
			goldQuantity -= wheatBuyPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough gold
		}
	}

	public void sellWheat()// *********
	{
		if(wheatBuyPrice > 0)
		{
			lessWheat();
			goldQuantity += wheatSellPrice;
            goldScore.text = goldQuantity.ToString();
		}
		else
		{
			//Pop up window, not enough wheat
		}
	}

	public void disableButtons()
	{
		
	}

	public void enableButtons()
	{

	}
}