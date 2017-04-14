using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds data for an individual player.
[System.Serializable]
public class Player
{
    // All of the private properies and their public accessors.
    #region Properties
	
	public string ipAddress;   // ipAddress of the player, used to identify connections for networking 
	public int    connectionID;   // Used to send a message to a player, linked with id number
    public int    playerIndex;
    public bool   isConnected = true;

	public	int playerAbility = -1;

    public string Name;        // The player's name as entered.
    public int LongestRoad = 1;
	public bool LongestRoadWinner = false;
	public bool LargestArmyWinner = false;
	public bool GameWinner = false;

    private int armies;         // The number of armies deployed by this player over the entire map.
    private int character;      // The character player has chosen to play as, 0 for no character.
    private int cities;         // The number of settlements upgraded to cities by this player over the entire map.
    private int gold;           // The amount of gold a player currently owns.
    private int roads;          // The number of roads built by this player over the entire map. 
    private int settlements;    // The number of settlements built by this player over the entire map. 
    private int turnOrder;      // The place in the turn order the player is, 0 for position not set.
    private int victoryPoints;  // The amount of victory points a player currently has.

    private float brickDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Brick from the market.
    private float oreDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Ore from the market.
    private float wheatDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Wheat from the market.
    private float woodDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Wood from the market.
    private float woolDiscount = Constants.NoDiscount; // The discount the player receives when purchasing Wool from the market.

    // Constructors to instantiate Player classes
    #region Public constructors

    // Default constructor.
    public Player()
    {
        Name = "";
        armies = 0;
        character = 0;
        cities = 0;
        gold = Constants.StartingGold;
        roads = 0;
        settlements = 0;
        turnOrder = 0;
        victoryPoints = 0;
    }

    // Constructor used in the game lobby.
    public Player(string NewName)
    {
        Name = NewName;
        armies = 0;
        character = 0;
        cities = 0;
        gold = Constants.StartingGold;
        roads = 0;
        settlements = 0;
        turnOrder = 0;
        victoryPoints = 0;
    }

    public Player(string NewName, int Ability)
    {
        Name = NewName;
        playerAbility = Ability;
        armies = 0;
        character = 0;
        cities = 0;
        gold = Constants.StartingGold;
        roads = 0;
        settlements = 0;
        turnOrder = 0;
        victoryPoints = 0;
    }
    #endregion Public constructors

    // Public accessors. Includes data validation.
    #region Public accessors

    public int Armies
    {
        get
        {
            return armies;
        }
        set
        {
            if (value > 0)
            {
                armies = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Armies cannot be less than zero.");
        }
	}

    public int Character
    {
        get
        {
            return character;
        }
        set
        {
            if (value >= 0)
            {
                character = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Character selection cannot be less than zero.");
        }
    }

    public int Cities
    {
        get
        {
            return cities;
        }
        set
        {
            if (value >= 0)
            {
                if (value <= Constants.MaxCities)
                    cities = value;
                else
                    throw new System.ArgumentOutOfRangeException("Cities built cannot exceed the maximum number of cities.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Cities built cannot be less than zero.");
        }
    }

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            if (value >= 0)
            {
                gold = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Gold amount cannot be less than zero.");
        }
    }

    public int Roads
    {
        get
        {
            return roads;
        }
        set
        {
            if (value >= 0)
            {
                if (value <= Constants.MaxRoads)
                    roads = value;
                else
                    throw new System.ArgumentOutOfRangeException("Roads built cannot exceed the maximum number of roads.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Roads built cannot be less than zero.");

        }
    }

    public int Settlements
    {
        get
        {
            return settlements;
        }
        set
        {
            if (value >= 0)
            {
                if (value <= Constants.MaxSettlements)
                    settlements = value;
                else
                    throw new System.ArgumentOutOfRangeException("Settlements built cannot exceed the maximum number of settlements.");
            }
            else
                throw new System.ArgumentOutOfRangeException("Settlements built cannot be less than zero.");

            }
    }

    public int TurnOrder
    {
        get
        {
            return turnOrder;
        }
        set
        {
            if (value > 0)
            {
                turnOrder = value;
            }
            throw new System.ArgumentOutOfRangeException("Turn order must be greater than zero.");
        }
    }

    public int VictoryPoints
    {
        get
        {
            return victoryPoints;
        }
        set
        {
            if (value >= 0)
            {
                victoryPoints = value;
            }
            else
                throw new System.ArgumentOutOfRangeException("Victory points cannot be less than zero.");
        }
    }

    // Public accessors to return the resources available.
    public int Brick { get; set;}
    public int Ore { get; set; }
    public int Wheat { get; set; }
    public int Wood { get; set; }
    public int Wool { get; set; }

    #endregion Public accessors
    #endregion Properties

    // Public methods.
    #region Methods

    // Sets the standard resource discount for when a player builds on a general port.
    public void SetStandardDiscount()
    {
        brickDiscount = Constants.StandardDiscount;
        oreDiscount = Constants.StandardDiscount;
        wheatDiscount = Constants.StandardDiscount;
        woodDiscount = Constants.StandardDiscount;
        woolDiscount = Constants.StandardDiscount;
    }

    // Sets the specific Brick discount for when a player builds on a Brick port.
	public void SetBrickDiscount() { brickDiscount = Constants.StandardDiscount; }

    // Sets the specific Ore discount for when a player builds on an Ore port.
	public void SetOreDiscount() { oreDiscount = Constants.StandardDiscount; }

    // Sets the specific Wheat discount for when a player builds on a Wheat port.
	public void SetWheatDiscount() { wheatDiscount = Constants.StandardDiscount; }

    // Sets the specific Wood discount for when a player builds on a Wood port.
	public void SetWoodDiscount() { woodDiscount = Constants.StandardDiscount; }

    // Sets the specific Wool discount for when a player builds on a Wool port.
	public void SetWoolDiscount() { woolDiscount = Constants.StandardDiscount; }


    public bool CanBuyBrick()
    {
        bool canBuy = true;

        float cost = brickDiscount * Constants.ResourceCost;
        
        if (Gold < (int)cost)
            canBuy = false;

        return canBuy;
    }

    // Subtracts the cost for a Brick from the player's gold and adds a Brick to their inventory.
    public void BuyBrick()
    {
        float cost = brickDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (Gold >= (int)cost)
        {
            Gold -= (int)cost;
            Brick++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Brick.");
    }

	public void SellBrick()
	{

		// If player has enough Brick, complete transaction, else throw exception.
		if (Brick > 0)
		{
			Gold += (int)Constants.ResourceCost;
			Brick--;
		}
		else
			throw new System.ArgumentOutOfRangeException("Player does not own Brick to sell.");
	}

    public bool CanBuyOre()
    {
        bool canBuy = true;

        float cost = oreDiscount * Constants.ResourceCost;
        
        if (Gold < (int)cost)
            canBuy = false;

        return canBuy;
    }

    // Subtracts the cost for an Ore from the player's gold and adds an Ore to their inventory.
    public void BuyOre()
    {
        float cost = oreDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (Gold >= (int)cost)
        {
            Gold -= (int)cost;
            Ore++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase an Ore.");
    }

	public void SellOre()
	{

		// If player has enough Ore, complete transaction, else throw exception.
		if (Ore > 0)
		{
			Gold += (int)Constants.ResourceCost;
			Ore--;
		}
		else
			throw new System.ArgumentOutOfRangeException("Player does not own Ore to sell.");
	}

    public bool CanBuyWheat()
    {
        bool canBuy = true;

        float cost = wheatDiscount * Constants.ResourceCost;
        
        if (Gold < (int)cost)
            canBuy = false;

        return canBuy;
    }

    // Subtracts the cost for a Wheat from the player's gold and adds a Wheat to their inventory.
    public void BuyWheat()
    {
        float cost = wheatDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (Gold >= (int)cost)
        {
            Gold -= (int)cost;
            Wheat++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Wheat.");
    }

	public void SellWheat()
	{

		// If player has enough Wheat, complete transaction, else throw exception.
		if (Wheat > 0)
		{
			Gold += (int)Constants.ResourceCost;
			Wheat--;
		}
		else
			throw new System.ArgumentOutOfRangeException("Player does not own Wheat to sell.");
	}

    public bool CanBuyWood()
    {
        bool canBuy = true;

        float cost = woodDiscount * Constants.ResourceCost;
        
        if (Gold < (int)cost)
            canBuy = false;

        return canBuy;
    }

    // Subtracts the cost for a Wood from the player's gold and adds a Wood to their inventory.
    public void BuyWood()
    {
        float cost = woodDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (Gold >= (int)cost)
        {
            Gold -= (int)cost;
            Wood++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Wood.");
    }

	public void SellWood()
	{

		// If player has enough Wood, complete transaction, else throw exception.
		if (Wood > 0)
		{
			Gold += (int)Constants.ResourceCost;
			Wood--;
		}
		else
			throw new System.ArgumentOutOfRangeException("Player does not own Wood to sell.");
	}

    public bool CanBuyWool()
    {
        bool canBuy = true;

        float cost = woolDiscount * Constants.ResourceCost;

        if (Gold < (int)cost)
            canBuy = false;

        return canBuy;
    }

    // Subtracts the cost for a Wool from the player's gold and adds a Wool to their inventory.
    public void BuyWool()
    {
        float cost = woolDiscount * Constants.ResourceCost;

        // If player has enough gold, complete transaction, else throw exception.
        if (Gold >= (int)cost)
        {
            Gold -= (int)cost;
            Wool++;
        }
        else
            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to purchase a Wool.");
    }

	public void SellWool()
	{

		// If player has enough Wool, complete transaction, else throw exception.
		if (Wool > 0)
		{
			Gold += (int)Constants.ResourceCost;
			Wool--;
		}
		else
			throw new System.ArgumentOutOfRangeException("Player does not own Wool to sell.");
	}

    // Adds gold to the player based on the amount of settlements and cities constructed.
    public void AddTurnGold()
    {
        int goldToAdd = 0;

        goldToAdd += (settlements * Constants.GoldPerSettlement);
        goldToAdd += (cities * Constants.GoldPerCity);

        gold += goldToAdd;
    }

    public bool CanBuildRoad()
    {
        bool tempBool = true;

		if (playerAbility != 0)
        	if (Brick < Constants.BricksPerRoad )
            	tempBool = false;
        if (Ore < Constants.OrePerRoad)
            tempBool = false;
        if (Wheat < Constants.WheatPerRoad)
            tempBool = false;
		if (playerAbility == 0)
	        if (Wood < 2 * Constants.WoodPerRoad)
	            tempBool = false;
		else
			if (Wood < Constants.WoodPerRoad)
				tempBool = false;
        if (Wool < Constants.WoolPerRoad)
            tempBool = false;

        return tempBool;
    }

    // Subtracts the cost for a Road from the player's resource inventory and adds a Road to items built.
    public void BuildRoad()
    {
		if (playerAbility != 0)
	        if (Brick < Constants.BricksPerRoad)
	            throw new System.ArgumentOutOfRangeException("Player does not own enough Brick to build a Road.");
        if (Ore < Constants.OrePerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Ore to build a Road.");
        if (Wheat < Constants.WheatPerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wheat to build a Road.");
		if (playerAbility == 0)
			if (Wood < 2 * Constants.WoodPerRoad)
	            throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a Road.");
		else
			if (Wood < Constants.WoodPerRoad)
				throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a Road.");
        if (Wool < Constants.WoolPerRoad)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wool to build a Road.");

		if (playerAbility != 0)
        	Brick -= Constants.BricksPerRoad;
        Ore -= Constants.OrePerRoad;
        Wheat -= Constants.WheatPerRoad;
		if (playerAbility == 0)
			Wood -= (2 * Constants.WoodPerRoad);
		else 
			Wood -= Constants.WoodPerRoad;
        Wool -= Constants.WoolPerRoad;

        Roads++;
    }

    // Subtracts the cost for a Settlement from the player's resource inventory and adds a Settlement to items built.
    public void BuildSettlement()
    {
        if (Brick < Constants.BricksPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Brick to build a Settlement.");
        if (Ore < Constants.OrePerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Ore to build a Settlement.");
        if (Wheat < Constants.WheatPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wheat to build a Settlement.");
        if (Wood < Constants.WoodPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a Settlement.");
        if (Wool < Constants.WoolPerSettlement)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wool to build a Settlement.");

        Brick -= Constants.BricksPerSettlement;
        Ore -= Constants.OrePerSettlement;
        Wheat -= Constants.WheatPerSettlement;
        Wood -= Constants.WoodPerSettlement;
        Wool -= Constants.WoolPerSettlement;

        Settlements++;

		UpdateVictoryPoints();
    }

    public bool CanBuildSettlement()
    {
        bool tempBool = true;

        if (Brick < Constants.BricksPerSettlement)
            tempBool = false;
        if (Ore < Constants.OrePerSettlement)
            tempBool = false;
        if (Wheat < Constants.WheatPerSettlement)
            tempBool = false;
        if (Wood < Constants.WoodPerSettlement)
            tempBool = false;
        if (Wool < Constants.WoolPerSettlement)
            tempBool = false;

        return tempBool;
    }

    // Subtracts the cost for a City from the player's resource inventory and adds a City to items built.
    public void BuildCity()
    {
        if (Brick < Constants.BricksPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Brick to build a City.");
        if (Ore < Constants.OrePerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Ore to build a City.");
        if (Wheat < Constants.WheatPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wheat to build a City.");
        if (Wood < Constants.WoodPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wood to build a City.");
        if (Wool < Constants.WoolPerCity)
            throw new System.ArgumentOutOfRangeException("Player does not own enough Wool to build a City.");

        Brick -= Constants.BricksPerCity;
        Ore -= Constants.OrePerCity;
        Wheat -= Constants.WheatPerCity;
        Wood -= Constants.WoodPerCity;
        Wool -= Constants.WoolPerCity;

		Cities++;

		UpdateVictoryPoints();
    }

    public bool CanBuildCity()
    {
        bool tempBool = true;

        if (Brick < Constants.BricksPerCity)
            tempBool = false;
        if (Ore < Constants.OrePerCity)
            tempBool = false;
        if (Wheat < Constants.WheatPerCity)
            tempBool = false;
        if (Wood < Constants.WoodPerCity)
            tempBool = false;
        if (Wool < Constants.WoolPerCity)
            tempBool = false;

        return tempBool;
    }

    public bool CanHireArmy()
    {
        bool canBuy = true;

        float cost = Constants.ArmyGoldCost;

		if (playerAbility != 1)
	        if (Gold < (int)cost)
	            canBuy = false;
		else
			if (Gold < (int)(.5f * cost))
				canBuy = false;

        return canBuy;
    }

    // Subtracts the cost to hire an Army from the player's gold amount and adds a Army to the player's total armies.
    public void HireArmy()
    {
		if (playerAbility != 1)
	        if (gold < Constants.ArmyGoldCost)
	            throw new System.ArgumentOutOfRangeException("Player does not own enough gold to hire an Army.");
		else 
			if (gold < (Constants.ArmyGoldCost / 2))
				throw new System.ArgumentOutOfRangeException("Player does not own enough gold to hire an Army.");

		if (playerAbility != 1)
        	gold -= Constants.ArmyGoldCost;
		else
			gold -= (Constants.ArmyGoldCost / 2);
        Armies++;

		UpdateVictoryPoints();
    }

	public void UpdateVictoryPoints()
	{
		int tempVP;

		tempVP = (Settlements * Constants.SettlementVP) + (Cities * Constants.CityVP);

		if (LongestRoadWinner)
			tempVP += Constants.LongestRoadVP;

		if (LargestArmyWinner)
			tempVP += Constants.LargestArmyVP;

		VictoryPoints = tempVP;
	}
	
    #endregion Methods
}


