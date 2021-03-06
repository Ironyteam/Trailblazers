﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds all constants for the entire project.
public static class Constants
{
   // Character select name integers
   // "Abiha the Exiled", "Rosa del Fuego"
   public const int NattyBumppo       = 0;
   public const int ScaryHarry        = 1;
   public const int Ganzo             = 2;
   public const int GamlyTheRed       = 3;
   public const int MaidenOfDunshire  = 4;
   public const int QueenApala        = 5;
   public const int AbihaTheExiled    = 6;
   public const int RosaDelFuego      = 7;

   // Network message dividers
   public const string commandDivider  = ":";
   public const string gameDivider     = "~";
   public const string gameListDivider = ";";

   // Server browser and network lobby commands
   public const string addGame              = "1";
   public const string addPlayer            = "2";
   public const string requestGameList      = "3";
   public const string cancelGame           = "4"; // LEAVE THE SAME FOR SERVER TO KNOW
   public const string goToCharacterSelect = "22";
   public const string inGameSceneLoaded   = "27";
   public const string goToInGameScene     = "28";
   public const string enterInGameScene    = "30";
   public const string sendGameInfo        = "24";
   public const string playerNumber        = "23";
   public const string playerDisconnect    = "25";
   public const string playerAdded         = "29";
   public const string lobbyFull           = "20";
   public const string leaveGame           = "26";
   public const string gameStarted          = "5";
   public const string gameEnded            = "6";
   public const string characterSelect      = "7";
   public const string characterResult      = "8";
   public const string diceRoll             = "9";

   // In game turn commands
   public const string buildSettlement     = "10";
   public const string upgradeToCity       = "11";
   public const string buildRoad           = "12";
   public const string buildArmy           = "13";
   public const string attackCity          = "14";
   public const string moveRobber          = "15";
   public const string endTurn             = "16";
   public const string startTurn           = "17";
   public const string sendChat            = "18";
   public const string networkError        = "19";
   public const string sendMap             = "21";
   public const string serverKillCode    = "1234";


    public const int StartingGold      = 150;
	public const int StartingResources = 0;
	public const int GoldPerSettlement = 50;
	public const int GoldPerCity	   = 50;

	public const int BricksPerRoad = 1;
	public const int OrePerRoad	   = 0;
	public const int WheatPerRoad  = 0;
	public const int WoodPerRoad   = 1;
	public const int WoolPerRoad   = 0;

	public const int BricksPerSettlement = 1;
	public const int OrePerSettlement    = 0;
	public const int WheatPerSettlement  = 1;
	public const int WoodPerSettlement   = 1;
	public const int WoolPerSettlement   = 1;

	public const int BricksPerCity = 0;
	public const int OrePerCity    = 3;
	public const int WheatPerCity  = 2;
	public const int WoodPerCity   = 0;
	public const int WoolPerCity   = 0;

	public const int ArmyGoldCost  = 100;
	
	public const int MaxRoads	    = 100; 
	public const int MaxSettlements = 100; 
	public const int MaxCities      = 100;
	public const int MaxPlayers     = 6; 
	
	
	public const int MinXCoordinate = 0;
	public const int MaxXCoordinate = 40;
	public const int MinYCoordinate = 0;
	public const int MaxYCoordinate = 40;

	public const float TurnTime     	 = 120.0f;
	public const float StandardDiscount  = 0.75f;
	public const float SpecificDiscount  = 0.50f;
	public const float NoDiscount 		 = 1.00f;
	public const float ResourceCost	 	 = 150.0f;
    public const float ResourceSalePrice = 75.0f;

    public const float QueenGoldBonus = 1.5f;
	public const int LongestRoadBonus = 2;
	public const int LargestArmyBonus = 4;
    public const int MinLargestArmy = 4;
    public const int MinLongestRoad = 4;

    public const int beginClip = 0;
	public const int lastClip  = 2;

	public const int LongestRoadVP	   = 3;
	public const int LargestArmyVP	   = 2;
	public const int CityVP			   = 2;
	public const int SettlementVP	   = 1;

	// Camera Constants
	public const float CAMERA_MIN_X = 5.299987f;
	public const float CAMERA_MAX_X = 24.14998f;
	public const float CAMERA_MIN_Z = 4.299974f;
	public const float CAMERA_MAX_Z = 21.84998f;
    
	// Chat Constants
	public const int MaxChatMessages = 14;
	public const int playerBasedMessageIndex = 12;
	public static readonly string[] chatMessages = {"Avenge me!", "I will conquer!", "Good Move.", "Now you lose!",
											 		"I did not expect that!", "No!", "Perfect!", "Good Game", "GG",
											 		"Watch This!", "Init to Win it!", " is here to Win!", "Blast!",
											 		"Ryuu ga waka teki wo kurao"};

	// Popup Constants
	public const float popupWaitTime = 2.0f;

    // Character select constants
    public const float characterSelectTimer = 30.0f;
}
