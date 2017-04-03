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
    public Text DiceText;
    public bool GameCanvasEnabled = true;

    [System.Serializable]
    public class playerIndicatorClassLocal
    {
		public bool largestArmyIndicator = false;
		public bool longestRoadIndicator = false;
		public string characterPictureName;
		public int uiPosition;
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
	}

    public InputField chatInput;
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
           int currentPlayer  = 0,
 			   playerNumberCurrent = 1;
    
    public playerIndicatorClassLocal[] playerClassLocalArray = new playerIndicatorClassLocal[MAX_NUM_OF_PLAYERS];
	public screenElements[] screenElementsArray = new screenElements[MAX_NUM_OF_PLAYERS];

    public GameBoard CurrentGameBoard;
    
    void Awake()
    {
        CurrentGameBoard = GameObject.Find("Map").GetComponent<GameBoard>();

        for (int count = 0; count < BoardManager.numOfPlayers; count++)
		{
			playerClassLocalArray[count].characterPictureName = Characters.Names[characterSelect.selectedCharacters[count]];
			playerClassLocalArray[count].uiPosition = count;
            screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite> (playerClassLocalArray[count].characterPictureName) as Sprite;
			screenElementsArray[count].largestArmyIndicator.enabled = false;
			screenElementsArray[count].longestRoadIndicator.enabled = false;
            screenElementsArray[count].characterSelected.enabled = false;
        }

        screenElementsArray[0].characterSelected.enabled = true;

        gameCanvas.enabled    = true;
	    escapeCanvas.enabled  = false;
		optionsCanvas.enabled = false;
		shopCanvas.enabled    = false;
		barracksCanvas.enabled = false;
        DiceImage.enabled = false;
        DiceText.enabled = false;
    }

	void Start()
	{
        for (int count = 0; count < MAX_NUM_OF_PLAYERS; count++)
        {
            if (count < BoardManager.numOfPlayers)
                screenElementsArray[count].playerCanvas.enabled = true;
            else
                screenElementsArray[count].playerCanvas.enabled = false;
        }
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

	    if (Input.GetKeyDown("s") && GameCanvasEnabled)
		{
			if(shopCanvas.enabled == false)
				shopCanvas.enabled = true;
			else
				shopCanvas.enabled = false;
		}

		if (Input.GetKeyDown("b") && GameCanvasEnabled)
		{
			if(barracksCanvas.enabled == false)
				barracksCanvas.enabled = true;
			else
				barracksCanvas.enabled = false;
		}
    }

    // Adds the passed string to the chat box. Used for both chat and system messages.
    public void AddChatMessage(int playerNumber, string message)
    {
        chatBox.text += '\n' + message;
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
            for (int count = 0; count < BoardManager.numOfPlayers; count++)
            {
                screenElementsArray[count].characterPicture.sprite = Resources.Load<Sprite>(playerClassLocalArray[(playerNumberCurrent + count) % BoardManager.numOfPlayers].characterPictureName) as Sprite;
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
            if (playerNumberCurrent < BoardManager.numOfPlayers)
                playerNumberCurrent++;
            else
                playerNumberCurrent = 1;

            if (currentPlayer < BoardManager.numOfPlayers - 1)
                currentPlayer++;
            else
                currentPlayer = 0;
        }
        else
        {
            // Code to reverse current player
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

        screenElementsArray[lastPlayer].characterSelected.enabled = false;
        screenElementsArray[CurrentGameBoard.CurrentPlayer].characterSelected.enabled = true;
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
}
