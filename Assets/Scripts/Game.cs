using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Holds game data for all game types on the local machine.
[System.Serializable]
public class Game
{
    #region Properties

    public List<Player> PlayerList = new List<Player>();  // Holds a list of all players in the game.

    public bool isNetwork = false;          // Either a local or network game.

    private string mapName;             // The name of the map chosen for this game to be played on.

    private bool gameOver;              // Is the game over or not.

    private int longestRoad = 4;            // The length of the longest road.
    private int longestRoadPlayer = -1;      // The player number who owns the longest road.
    private int mostArmies = 7;             // The size of the largest army.
    private int mostArmiesPlayer = -1;       // The player number who owns the largest army.
    private int victoryPointsToWin = 10;     // Winning condition for the game. Will be adjusted based on the map chosen.

    #region Public constructors

    // Default constructor.
    public Game()
    {

    }

    #endregion Public constructors

    #region Public accessors

    public string MapName { get; set; }

    public bool GameOver { get; set; }

    public int LongestRoad
    {
        get
        {
            return longestRoad;
        }
        set
        {
            if (value > 0)
            {
                if (value <= Constants.MaxRoads)
                    longestRoad = value;
                else
                    throw new System.ArgumentOutOfRangeException("Longest road cannot be more than the maximum number of roads.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Longest road cannot be less than zero.");
        }
	}

    public int LongestRoadPlayer
    {
        get
        {
            return longestRoadPlayer;
        }
        set
        {
            if (value >= 0)
            {
                if (value < PlayerList.Count)
                    longestRoadPlayer = value;
                else
                    throw new System.ArgumentOutOfRangeException("Longest road player cannot be more than the maximum number of players in game.");
            }
                else
                    throw new System.ArgumentOutOfRangeException("Longest road player cannot be less than zero.");
        }
    }
	
    public int MostArmies
    {
        get
        {
            return mostArmies;
        }
        set
        {
            if (value > 0)
            {
                mostArmies = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Most armies cannot be less than zero.");
        }
	}
	
	public int MostArmiesPlayer
    {
        get
        {
            return mostArmiesPlayer;
        }
        set
        {
            if (value > 0)
            {
                if (value < PlayerList.Count)
                    mostArmiesPlayer = value;
                else
                    throw new System.ArgumentOutOfRangeException("Most armies player cannot be more than the maximum number of players in game.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Most armies player cannot be less than zero.");
        }
	}
	
	public int VictoryPointsToWin
    {
        get
        {
            return victoryPointsToWin;
        }
        set
        {
            if (value > 0)
            {
                victoryPointsToWin = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Victory points to win must be greater than zero.");
        }
    }
	
	#endregion Public accessors
	#endregion Properties
}






