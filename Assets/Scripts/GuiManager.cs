using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {
    
    public const int LOCAL_PLAYER       = 0;   
    public const int MAX_NUM_OF_PLAYERS = 6;
    
    [System.Serializable]
    public class playerIndicatorClass
    {
        public Image largestArmyIndicator;
        public Image longestRoadIndicator;
		public Image characterPicture;
        public Text armySizeDisplay;
        public Text largestArmyAmount;
        public Text longestRoadAmount;
        public Text numOfCitiesDisplay;
        public Text numOfSettlementsDisplay;
        public Text victoryPoints;
        public string playerName;
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
    
    public playerIndicatorClass[] playerIndicatorArray = new playerIndicatorClass[MAX_NUM_OF_PLAYERS];    
    
    // Sets the gold for the local player.
    public void SetPlayerGold(int goldAmount)
    {
        goldScore.text = goldAmount.ToString();
    }

    // Sets the longest road length for the local player.
    public void SetLongestRoad(int largestNumOfConsecutiveRoads)
    {
        playerIndicatorArray[LOCAL_PLAYER].longestRoadAmount.text = largestNumOfConsecutiveRoads.ToString();
    }

    // Sets the longest road length shown for all other players, based on passed player number.
    public void SetLongestRoad(int largestNumOfConsecutiveRoads, int playerNumber) // Need number for previous player with longest road
    {
        playerIndicatorArray[playerNumber].longestRoadAmount.text = largestNumOfConsecutiveRoads.ToString();
    }

    // Sets the army size for the local player.
    public void SetArmySize(int armySize)
    {
        playerIndicatorArray[LOCAL_PLAYER].armySizeDisplay.text = armySize.ToString();
    }

    // Sets the army size shown for all other players, based on passed player number.
    public void SetArmySize(int armySize, int playerNumber)
    {
        playerIndicatorArray[playerNumber].armySizeDisplay.text = armySize.ToString();
    }

    // Sets the Victory Points shown for local player.
    public void SetPlayerVP(int victoryPointAmount)
    {
        playerIndicatorArray[LOCAL_PLAYER].victoryPoints.text = victoryPointAmount.ToString();
    }

    // Sets the Victory Points shown for all other players, based on passed player number.
    public void SetPlayerVP(int victoryPointAmount, int playerNumber)
    {
        playerIndicatorArray[playerNumber].victoryPoints.text = victoryPointAmount.ToString();
    }

    // Sets the dice to the passed dice roll.
    public void RollDice(int diceRoll)
    {
        // Code to animate the dice roll and show the rolled amount passed.
    }

    // Adds the passed string to the chat box. Used for both chat and system messages.
    public void AddChatMessage(int playerNumber, string message)
    {
        chatBox.text += '\n' + playerIndicatorArray[playerNumber].playerName + ": " + message;
    }

    // Removes a road from roads remaining, and adds a road to roads built for the local player.
    public void BuildRoad()
    {
     // Code to remove a road from roads remaining, and add a road to roads built for the local player. No other player option needed.
    }

    // Removes a settlement from settlements remaining, and adds a settlement to settlements built for the local player.
    public void BuildSettlement(int numOfSettlements)
    {
     // Code to remove a settlement from settlements remaining, and add a settlement to settlements built for the local player.
        playerIndicatorArray[LOCAL_PLAYER].numOfSettlementsDisplay.text = numOfSettlements.ToString();
    }

    // Removes a settlement from settlements remaining, and adds a settlement to settlements built for all other players, based on passed player number.
    public void BuildSettlement(int numOfSettlements, int playerNumber)
    {
     // Code to remove a settlement from settlements remaining, and add a settlement to settlements built for all other players, based on passed player number.
        playerIndicatorArray[playerNumber].numOfSettlementsDisplay.text = numOfSettlements.ToString();       
    }

    // Removes a city from cities remaining, and adds a city to cities built for the local player.
    public void BuildCity(int numOfCities)
    {
     // Code to remove a city from cities remaining, and add a city to cities built for the local player.
        playerIndicatorArray[LOCAL_PLAYER].numOfCitiesDisplay.text = numOfCities.ToString();
    }

    // Removes a city from cities remaining, and adds a city to cities built for all other players, based on passed player number.
    public void BuildCity(int numOfCities, int playerNumber)
    {
     // Code to remove a city from cities remaining, and add a city to cities built for all other players, based on passed player number.
        playerIndicatorArray[playerNumber].numOfCitiesDisplay.text = numOfCities.ToString(); 
    }

    // Sets the local player to the current winner of the longest road.
    public void SetLongestRoadWinner(int previousLongestRoad)
    {
     // Code to the local player to the current winner of the longest road.
        if(previousLongestRoad > -1)
            playerIndicatorArray[previousLongestRoad].longestRoadIndicator.enabled = false;
        
        playerIndicatorArray[LOCAL_PLAYER].longestRoadIndicator.enabled = true;
    }

    // Sets the passed player number to the current winner of the longest road.
    public void SetLongestRoadWinner(int previousLongestRoad, int playerNumber)
    {
     // Code to set the passed player number to the current winner of the longest road.
        if(previousLongestRoad > -1)
            playerIndicatorArray[previousLongestRoad].longestRoadIndicator.enabled = false;
        
        playerIndicatorArray[playerNumber].longestRoadIndicator.enabled = true;
    }

    // Sets the local player to the current winner of the largest army.
    public void SetLargestArmyWinner(int previousLargestArmy)
    {
     // Code to set the local player to the current winner of the largest army.
             if(previousLargestArmy > -1)
            playerIndicatorArray[previousLargestArmy].largestArmyIndicator.enabled = false;
        
        playerIndicatorArray[LOCAL_PLAYER].largestArmyIndicator.enabled = true;
    }

    // Sets the passed player number to the current winner of the largest army.
    public void SetLargestArmyWinner(int previousLargestArmy, int playerNumber)
    {
     // Code to set the passed player number to the current winner of the largest army.
        if(previousLargestArmy > -1)
            playerIndicatorArray[previousLargestArmy].largestArmyIndicator.enabled = false;
        
        playerIndicatorArray[playerNumber].largestArmyIndicator.enabled = true;
    }

    // Updates the timer display with the passed amount of time remaining.
    public void UpdateTimer(int timeLeft)
    {
     // Code tp update the timer. For now just create a text box that will display the time in seconds remaining.
    }

	public void swapPlayerImage(int playerNumber)
	{
		
	}
}
