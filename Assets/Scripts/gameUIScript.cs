using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class gameUIScript : MonoBehaviour {
	public const int MAX_PLAYERS = 5;
    public Canvas gameCanvas,
   	              escapeCanvas,
		          optionsCanvas,
				  player1Canvas,
				  player2Canvas,
				  player3Canvas,
				  player4Canvas,
				  player5Canvas,
				  player6Canvas;
    public InputField chatInput;
           int brickQuantity  = 0,
               goldQuantity   = 100,
               oreQuantity    = 0,
           	   sheepQuantity  = 0,
           	   tempEscPressed = 0, 
          	       // Will be removed once i figure out how getKeyDown really works
           	   wheatQuantity  = 0,
           	   woodQuantity   = 0;
	public int numOfPlayers,
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
	public Image characterImage1;
	public Image characterImage2;
	public Image characterImage3;
	public Image characterImage4;
	public Image characterImage5;
	public string[] imageNames = new string[5] {"Jonas", "Archer", "Miller_S", "Miller_L", "Teed"};
	public int imageNumberCurrent = 1;

    void Awake()
    {
	    gameCanvas.enabled = true;
	    escapeCanvas.enabled = false;
		optionsCanvas.enabled = false;
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
            if(tempEscPressed == 0)
            {
                escapeCanvas.enabled = true;
                tempEscPressed = 1;
            }
            else
            {
                escapeCanvas.enabled = false;
                tempEscPressed = 0;
            }
        }

		if(numOfPlayers >= 1)
		{
			player1Canvas.enabled = true;
			if(numOfPlayers >= 2)
			{		
				player2Canvas.enabled = true;
				if(numOfPlayers >= 3)
				{
					player3Canvas.enabled = true;
					if(numOfPlayers >=4)
					{
						player4Canvas.enabled = true;
						if(numOfPlayers >=5)
						{
							player5Canvas.enabled = true;
							if(numOfPlayers >=6)
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
        if(numOfPlayers < MAX_PLAYERS)
            numOfPlayers++;
				
	}

	public void removePlayers()
	{
		if(numOfPlayers > 0)
			numOfPlayers--;
	}

	public void swapPlayerImage()
	{
     //   if(numOfPlayers <= 2)
    //    {
            characterImage1.sprite = Resources.Load<Sprite> (imageNames[imageNumberCurrent % 5]) as Sprite;
            Debug.Log(imageNumberCurrent % 5);
            characterImage2.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 1) % 5]) as Sprite;
            Debug.Log((imageNumberCurrent + 1) % 5);
  //      }
 //       if(numOfPlayers <= 3)
   //     {
            characterImage3.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 2) % 5]) as Sprite;
            Debug.Log((imageNumberCurrent + 2) % 5);
  //      }
  //      if(numOfPlayers <= 4)
  //      {
		characterImage4.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 3) % 5]) as Sprite;
		Debug.Log((imageNumberCurrent + 3) % 5);
 //       }
//        if(numOfPlayers <= 5)
//        {
            characterImage5.sprite = Resources.Load<Sprite> (imageNames[(imageNumberCurrent + 4) % 5]) as Sprite;
            Debug.Log((imageNumberCurrent + 4) % 5);
 //       }
        
		imageNumberCurrent++;
		if(imageNumberCurrent > 5)
		{
			imageNumberCurrent = 1;
		}
	}
}
