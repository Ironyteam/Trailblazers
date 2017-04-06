﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
#region Variables
   int myReliableChannelId;
   int socketId;
   int socketPort = 5010;
   int connectionId;
   int serverConnectionID       = 1; // Updates when you connect to a server
   const int hostConnectionID   = 2;
   const string myIP = "127.0.0.1";
   public List<Player> lobbyPlayers = new List<Player>();
   public Player myPlayer;

   bool isHostingGame = false;
   public static bool inPlayerLobby   = false;
   bool inGame        = false;

   List<string[]> gameList = new List<string[]>();
   public GameObject gameInfoPanel;
   public GameObject gameListCanvas;
   public GameObject playerInfoPanel;
   public NetworkGame myGame;
   public GameBoard mapObject; // Used to find Luke's game script GameObject

   public Text messageLog;
   public Text ipField;

   // Buttons for Network Lobby scene
   public Button connectServerBTN;
   public Button hostGameBTN;
   public Button refreshGameListBTN;
   public Button cancelHostingBTN;
   public Button startGameBTN;
   public Button createGameBTN;
   public Button startGameCharacterSelectBTN;
   public characterSelect characterScript;

   // Game settings input fields
   public Text gameName;
   public Text maxPlayers;
   public Text gamePassword;
   public Text mapName;

   public string gameNameTemp = "Default";
   public string numberOfPlayersTemp = "0";
   public string maxPlayersTemp = "0";
   public string passwordTemp = "Default";
   public string mapNameTemp = "Default";

#endregion

   // Awake when object first created
   void Awake()
   {
      DontDestroyOnLoad(this.gameObject);

      if (FindObjectsOfType(GetType()).Length > 1)
      {
         Destroy(gameObject);
      }
   }

   // Use this for initialization
   void Start()
   {
      int maxConnections = 5;

      // Opens a port on local computer that will be used for sending and recieving, done on client and server
      NetworkTransport.Init();
      ConnectionConfig config = new ConnectionConfig();
      myReliableChannelId = config.AddChannel(QosType.Reliable);
      HostTopology topology = new HostTopology(config, maxConnections);
      socketId = NetworkTransport.AddHost(topology, socketPort);
      Debug.Log("\n" + "Socket open. Socket ID is : " + socketId);

		// Create an fake game on launch for testing TEST
      requestGameList("172.16.51.127~Name~4~5~password~map");
      myGame = new NetworkGame();
      myPlayer = new Player("DefaultNAME");
      lobbyPlayers.Add(myPlayer);
      lobbyPlayers.Add(myPlayer);
      lobbyPlayers.Add(myPlayer);
   }

   private void OnEnable()
   {
      SceneManager.sceneLoaded += hookUpLobbyFunctionality;
   }

   private void OnDisable()
   {
      SceneManager.sceneLoaded -= hookUpLobbyFunctionality;
   }

   private void hookUpLobbyFunctionality(Scene scene, LoadSceneMode mode)
   {
      if (scene.name == "Network Lobby")
      {
         // UI linkups, panels and fields
         gameInfoPanel = Resources.Load("GameInfoPNL") as GameObject;
         gameListCanvas = GameObject.Find("ContentPNL");
         playerInfoPanel = Resources.Load("PlayerInfoPNL") as GameObject;
         messageLog = GameObject.Find("MessageLogTXT").GetComponent<UnityEngine.UI.Text>();
         ipField = GameObject.Find("ipTXT").GetComponent<UnityEngine.UI.Text>();

         // Buttons linkup
         connectServerBTN = GameObject.Find("ServerBTN").GetComponent<Button>();
         connectServerBTN.onClick.AddListener(() => connectToServer());
         //hostGameBTN = GameObject.Find("HostGameBTN").GetComponent<Button>();
         //hostGameBTN.onClick.AddListener(() => hostGame());
         refreshGameListBTN = GameObject.Find("RefreshBTN").GetComponent<Button>();
         refreshGameListBTN.onClick.AddListener(() => requestGameListServer());
         cancelHostingBTN = GameObject.Find("CancelHostingBTN").GetComponent<Button>();
         cancelHostingBTN.onClick.AddListener(() => cancelGame());
         createGameBTN = GameObject.Find("CreateGameBTN").GetComponent<Button>();
         createGameBTN.onClick.AddListener(() => createGame());
         startGameBTN = GameObject.Find("StartGameBTN").GetComponent<Button>();
         startGameBTN.onClick.AddListener(() => startCharacterSelect());
         startGameBTN.gameObject.SetActive(false);
         if (!isHostingGame)
         {
            startGameBTN.gameObject.SetActive(false);
         }
         else
         {
            // Destroy the list of network games in the panel
            foreach (Transform child in gameListCanvas.transform)
            {
               GameObject.Destroy(child.gameObject);
            }

            //startGameBTN.gameObject.SetActive(true);
            createGameBTN.gameObject.SetActive(false);
            refreshGameListBTN.gameObject.SetActive(false);
            connectServerBTN.gameObject.SetActive(false);
         }

         GameObject.Find("Network Handler").GetComponent<NetworkManager>();
         Debug.Log("Loaded Network Lobby");
      }
      else if (scene.name == "In Game Scene")
      {
         mapObject = GameObject.Find("Map").GetComponent<GameBoard>();
         mapObject.LocalGame.isNetwork = true;
      }
      else if (scene.name == "Character Select")
      {
         startGameCharacterSelectBTN = GameObject.Find("Start Game").GetComponent<Button>();
         startGameCharacterSelectBTN.onClick.AddListener(() => characterSelectStartGame());
         characterScript = GameObject.Find("Main Camera").GetComponent<characterSelect>();
         if (!isHostingGame)
            startGameCharacterSelectBTN.enabled = false;
      }
   }

   // Connect to the server
   public void connectToServer()
   {
      byte error;
      Debug.Log("\n" + "Connecting to server: " + ipField.text);
      serverConnectionID = NetworkTransport.Connect(socketId, ipField.text, socketPort, 0, out error);
      connectServerBTN.gameObject.SetActive(false);
      ipField.transform.parent.gameObject.SetActive(false);
      Debug.Log("\n" + "ConnectionID: " + connectionId);
   }

   // Connect to ipAddress, target auto connects in return
   public void connectToGame(string ipAddress)
   {
      byte error;
      Debug.Log("\n" + "Trying to connect to: " + ipAddress);
      connectionId = NetworkTransport.Connect(socketId, ipAddress, socketPort, 0, out error);
      Debug.Log("\n" + "ConnectionID: " + connectionId);
   }

   // Connect to game test with it returning connection id
   public int connectToHost(string ipAddress)
   {
      byte error;
      Debug.Log("\n" + "Trying to connect to: " + ipAddress);
      int hostId = NetworkTransport.Connect(socketId, ipAddress, socketPort, 0, out error);
      Debug.Log("\n" + "ConnectionID: " + connectionId);
      return hostId;
   }


   // Create a empty game
   public void createGame()
   {
      isHostingGame = true;
   }

   // Set the values recieved from BoardManager and send the server the game
   public void setupGameSettings(int totalPlayers, int turnTimer, int victoryPoints, string gameName, string mapName, HexTemplate map)
   {
      myGame.maxPlayers = totalPlayers.ToString();
      myGame.turnTimer  = turnTimer.ToString();
      myGame.numberOfVictoryPoints = victoryPoints.ToString();
      myGame.gameName   = gameName;
      myGame.mapName    = mapName;
      myGame.gameMap    = map;
      myGame.mapString  = map.ToFileString();
      myGame.mapPiece1  = myGame.mapString.Substring(0, 400);
      myGame.mapPiece2  = myGame.mapString.Substring(400, 400);
      myGame.mapPiece3  = myGame.mapString.Substring(800, 400);
      myGame.mapPiece4  = myGame.mapString.Substring(1200, myGame.mapString.Length - 1200);
      hostGame();
      Debug.Log("Setting up game" + gameName + totalPlayers.ToString() + turnTimer.ToString() + victoryPoints.ToString());
   }

   // Send game info to the server
   public void hostGame()
   {
      if (isHostingGame)
      {
         string gameInfo;
         isHostingGame = true;
         myPlayer.connectionID = 0;
         lobbyPlayers.Add(myPlayer);

         gameInfo = Constants.addGame + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myGame.gameName +
         Constants.gameDivider + "0" + Constants.gameDivider + myGame.maxPlayers + Constants.gameDivider + myGame.password + Constants.gameDivider + myGame.mapName;
         sendSocketMessage(gameInfo, serverConnectionID);
      }
      else
      {
         Debug.Log("\nError: Already hosting a game.");
      }
   }

   // Go to character select and tell the clients to go there
   public void startCharacterSelect()
   {
      BoardManager.template          = myGame.gameMap;
      BoardManager.numOfPlayers      = Int32.Parse(myGame.maxPlayers);
      BoardManager.localPlayerIndex = myPlayer.playerIndex;
      sendActionToClients(Constants.goToCharacterSelect + Constants.commandDivider + Network.player.ipAddress, 0);
      sendPlayerNumbers();
      SceneManager.LoadScene("Character Select");
   }

   // Send all players what index they are on the player list
   public void sendPlayerNumbers()
   {
      for (int i = 1; i < lobbyPlayers.Count; i++)
      {
         sendSocketMessage(Constants.playerNumber + Constants.commandDivider + i, lobbyPlayers[i].connectionID);
      }
   }

   // Create a player list and add myself in the correct index position
   public void setupPlayersOnClient(int myIndex)
   {
      Debug.Log("Max players " + myGame.maxPlayers + " -  My index " + myIndex);
      for (int i = 0; i < Int32.Parse(myGame.maxPlayers); i++)
      {
         Player player = new Player();
         lobbyPlayers.Add(player);
      }
      myPlayer.playerIndex = myIndex;
      lobbyPlayers[myIndex] = myPlayer;
   }

   // A player either left of joined a game refresh the players panel
   public void lobbyPlayerChange(int numPlayers)
   {
      for (int i = 1; i < numPlayers; i++)
      {
         GameObject newPlayer = Instantiate(playerInfoPanel, gameListCanvas.transform, false);
      }
   }

   // Tell the clients to go to in game scene
   public void characterSelectStartGame()
   {
      inGame = true;
      sendActionToClients(Constants.gameStarted + Constants.commandDivider + Network.player.ipAddress, 0);
   }

   // Send a socket message to connectionId
   public void sendSocketMessage(string message, int connectionNum)
   {
      byte error;
      byte[] buffer = new byte[1024];
      Stream stream = new MemoryStream(buffer);
      BinaryFormatter formatter = new BinaryFormatter();

      formatter.Serialize(stream, message);

      int bufferSize = 1024;
      Debug.Log("\nSending to ID " + connectionNum + " : " + message);
      NetworkTransport.Send(socketId, connectionNum, myReliableChannelId, buffer, bufferSize, out error);
   }

   // Called every frame
   void Update()
   {
      int recHostId;
      int recConnectionId;
      int recChannelId;
      byte[] recBuffer = new byte[1024];
      int bufferSize = 1024;
      int dataSize;
      byte error;
      NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);


      switch (recNetworkEvent)
      {
         case NetworkEventType.Nothing:
            break;
         case NetworkEventType.ConnectEvent:
            Debug.Log("\n" + "Incoming connection event received");
            break;
         case NetworkEventType.DataEvent:
            Stream stream = new MemoryStream(recBuffer);
            BinaryFormatter formatter = new BinaryFormatter();
            string message = formatter.Deserialize(stream) as string;
            Debug.Log("\nIncoming: " + message);
            processNetworkMessage(message, recConnectionId);
            break;
         case NetworkEventType.DisconnectEvent:
            Debug.Log("\n" + "Remote client event disconnected");
            playerDisconnected(recConnectionId);
            break;
      }
   }

   public void processNetworkMessage(string networkMessage, int recConnectionID)
   {
      string[] gameInfo = networkMessage.Split(Convert.ToChar(Constants.commandDivider));

      switch (gameInfo[0])
      {
         case Constants.addPlayer:         // #, ipAddress, password
            addPlayer(gameInfo[1], recConnectionID);
            Debug.Log("\nAdding Player");
            break;
         case Constants.playerEvent:
            lobbyPlayerChange(Int32.Parse(gameInfo[1]));
            break;
         case Constants.requestGameList:   // #, game, game, game, game...
            requestGameList(gameInfo[1]);
            break;
         case Constants.cancelGame:        // #
            removeGame(gameInfo[1]);
            break;
         case Constants.sendMap:
            getMapFromNetwork(gameInfo[1]);
            break;
         case Constants.sendGameInfo:
            setupGameInfoFromHost(gameInfo[1]);
            break;
         case Constants.goToCharacterSelect:       // #, ipAddress
            Debug.Log("MapGame template is: " + myGame.gameMap);
            BoardManager.template         = myGame.gameMap;
            BoardManager.localPlayerIndex = myPlayer.playerIndex;
            SceneManager.LoadScene("Character Select");
            break;
         case Constants.characterResult:   // #, characterResult
            break;
         case Constants.gameStarted:
            SceneManager.LoadScene("In Game Scene");
            break;
         case Constants.gameEnded:         // #, ipAddress
            break;
         case Constants.playerNumber:
            setupPlayersOnClient(Int32.Parse(gameInfo[1]));
            break;
         case Constants.networkError:      // #, info
            networkError(gameInfo[1]);
            break;
         case Constants.diceRoll:          // #, number1, number2
            // Fall through
         case Constants.characterSelect:   // #, character, player index
            // Fall through
         case Constants.buildSettlement:
            // Fall through
         case Constants.upgradeToCity:
            // Fall through
         case Constants.buildRoad:
            // Fall through
         case Constants.buildArmy:
            // Fall through
         case Constants.attackCity:
            // Fall through
         case Constants.moveRobber:
            // Fall through
         case Constants.endTurn:
            // Fall through
         case Constants.startTurn:
            // Fall through
         case Constants.sendChat:
            gameCommandProcessing(gameInfo[0], gameInfo[1], recConnectionID);
            break;
         default:
            networkError(gameInfo[1]);
            break;
      }
   }

   // Send request to host to join game
   public void requestGameJoin(int hostId)
   {
      Debug.Log("\nRequesting to join game, connectionID:" + connectionId);
      string message = Constants.addPlayer + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myPlayer.Name;
      sendSocketMessage(message, hostId);
   }

   // Player disconnected from the game
   public void playerDisconnected(int playerConnectionID)
   {
      Player player = lobbyPlayers.Find(item => item.connectionID > 20);
      
   }

   // Joined game, disable buttons
   public void onJoinGameClient()
   {
      inPlayerLobby = true;

      /*// Destroy everything in the panel
      foreach (Transform child in gameListCanvas.transform)
      {
         child.gameObject.SetActive(false);
      }*/

      createGameBTN.gameObject.SetActive(false);
      refreshGameListBTN.gameObject.SetActive(false);
      cancelHostingBTN.gameObject.SetActive(false);
   }

   // Player connecting to your lobby
   public void addPlayer(string gameInfo, int connectionID)
   {
      Debug.Log("\nInside add player");
      string[] playerInfo = gameInfo.Split(Convert.ToChar(Constants.gameDivider));

      // Send the player the game info
      sendGameInfo(connectionID);

      // Send the player the map that is being played
      sendMap(myGame, connectionID);


      // Tell connecting player he failed to to join lobby, either it is full or we are not hosting
      if (!isHostingGame || !myGame.addPlayer())
      {
         lobbyFull(connectionID);
         return;
      }

      // Create player class
      Player newplayer = new Player()
      {
         connectionID = connectionID,
         ipAddress    = gameInfo,
         playerIndex  = lobbyPlayers.Count
      };

      lobbyPlayers.Add(newplayer);
      if (lobbyPlayers.Count == Int32.Parse(myGame.maxPlayers))
      {
         startGameBTN.gameObject.SetActive(true);
      }

      // Create UI GameObject to list player
      GameObject newPlayer = Instantiate(playerInfoPanel, gameListCanvas.transform, false);
      Text[] playerTexts = newPlayer.GetComponentsInChildren<Text>();
      playerTexts[0].text = myGame.numberOfPlayers;
      playerTexts[1].text = playerInfo[1];
      playerTexts[3].text = playerInfo[0];

      // Send the player join event to all the players
      sendActionToClients(Constants.playerEvent + Constants.commandDivider + lobbyPlayers.Count, -1);
   }

   // Send the player the game info
   public void sendGameInfo(int playerID)
   {
      string message = Constants.sendGameInfo + Constants.commandDivider + myGame.mapName + Constants.gameDivider +
                       myGame.maxPlayers + Constants.gameDivider + myGame.turnTimer + Constants.gameDivider + myGame.numberOfVictoryPoints +
                       Constants.gameDivider + myGame.abilitiesOn;
      sendSocketMessage(message, playerID);
   }

   // Assign the values to myGame recieved from host, also assign them to BoardManager's static variables
   public void setupGameInfoFromHost(string gameInfo)
   {
      // MapName, MaxPlayers, TurnTimer, VictoryPoints, AbilitesOn
      string[] parameters = gameInfo.Split(Convert.ToChar(Constants.gameDivider));

      myGame.mapName               = parameters[0];
      myGame.maxPlayers            = parameters[1];
      myGame.turnTimer             = parameters[2];
      myGame.numberOfVictoryPoints = parameters[3];
      myGame.abilitiesOn           = parameters[4];


      BoardManager.numOfPlayers         = Int32.Parse(myGame.maxPlayers);
      BoardManager.turnTimerMax         = Int32.Parse(myGame.turnTimer);
      BoardManager.victoryPoints        = Int32.Parse(myGame.numberOfVictoryPoints);
      if (Int32.Parse(myGame.abilitiesOn) == 1)
         BoardManager.characterAbilitiesOn = true;
      else
         BoardManager.characterAbilitiesOn = false;

   }

   // Send the map over the network as a string
   public void sendMap(NetworkGame game, int connectionID)
   {
      sendSocketMessage(Constants.sendMap + Constants.commandDivider + "1" + Constants.gameDivider + game.mapPiece1, connectionID);
      sendSocketMessage(Constants.sendMap + Constants.commandDivider + "2" + Constants.gameDivider + game.mapPiece2, connectionID);
      sendSocketMessage(Constants.sendMap + Constants.commandDivider + "3" + Constants.gameDivider + game.mapPiece3, connectionID);
      sendSocketMessage(Constants.sendMap + Constants.commandDivider + "4" + Constants.gameDivider + game.mapPiece4, connectionID);
   }

   // Save the map recieved from network to local machine
   public void getMapFromNetwork(string mapInfo)
   {
      string[] mapStuff = mapInfo.Split(Convert.ToChar(Constants.gameDivider));
      switch (mapStuff[0])
      {
         case "1":
            myGame.mapPiece1 = mapStuff[1];
            break;
         case "2":
            myGame.mapPiece2 = mapStuff[1];
            break;
         case "3":
            myGame.mapPiece3 = mapStuff[1];
            break;
         case "4":
            myGame.mapPiece4 = mapStuff[1];
            myGame.mapString = myGame.assembledMapStrings();
            // Save map to custom maps folder
            FileHandler fh = new FileHandler();
            fh.checkForFiles();

            if (fh.mapExists(myGame.mapName))
            {
               int numberToAppend = fh.findNumberToAppendToName(myGame.mapName, 1000);
               if (numberToAppend >= 0)
               {
                  int endOfFirstLine = myGame.mapString.IndexOf("\r\n");
                  myGame.mapName += "(" + numberToAppend + ")";
                  myGame.mapString = myGame.mapName + myGame.mapString.Substring(endOfFirstLine, myGame.mapString.Length - endOfFirstLine);
                  Debug.Log("SAVING MAP: " + myGame.mapString);
                  fh.saveMap(myGame.mapString);
               }
               else
                  Debug.Log("Error saving map from network.");
            }
            else
               fh.saveMap(myGame.mapString);

            Debug.Log("MAP SAVED");
            myGame.gameMap = fh.retrieveMap(myGame.mapName, false);
            break;
      }
   }

   // Player connecting to full or canceled lobby
   public void lobbyFull(int connectionID)
   {
      string message = Constants.lobbyFull + Constants.commandDivider + Network.player.ipAddress;
      sendSocketMessage(message, connectionID);
   }

   // Ask server to send list of games
   public void requestGameListServer()
   {
      sendSocketMessage(Constants.requestGameList + Constants.commandDivider + myIP, serverConnectionID);
   }

   // Updates the local game list with list from server
   void requestGameList(string serverGameList)
   {
      gameList.Clear();
      string[] gameInfo = serverGameList.Split(Convert.ToChar(Constants.gameListDivider));
      Debug.Log("\n" + "Adding Games\n");
      foreach (string game in gameInfo)
      {
         string[] tempGame = game.Split(Convert.ToChar(Constants.gameDivider));
         foreach (string item in tempGame)
         {
            Debug.Log(item);
         }
         gameList.Add(tempGame);
      }
      refreshGameList();
   }

   // Stop game from hosting if server sends a kill command
   public void removeGame(string gameInfo)
   {
      if (gameInfo == Constants.serverKillCode)
      {
         isHostingGame = false;
         Debug.Log("\n Your game has been canceled by the server");
      }
   }

   public void refreshGameList()
   {
      foreach (Transform child in gameListCanvas.transform)
      {
         GameObject.Destroy(child.gameObject);
      }

      foreach (string[] game in gameList)
      {
         GameObject serverGame = Instantiate(gameInfoPanel) as GameObject;
         serverGame.transform.SetParent(gameListCanvas.transform, false);
         Text[] nameText = serverGame.GetComponentsInChildren<Text>();
         nameText[0].text = game[1];
         nameText[1].text = game[2];
         nameText[2].text = "\\" + game[3];
         nameText[3].text = game[4];
         nameText[4].text = game[5];
         nameText[6].text = game[0];
      }
   }

   // Tell the server that game is canceled
   public void cancelGame()
   {
      if (isHostingGame)
      {
         isHostingGame = false;
         startGameBTN.gameObject.SetActive(false);
         sendSocketMessage(Constants.cancelGame + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);
      }
   }

   // Tell the server the game has started
   void gameStarted(string[] gameInfo)
   {

   }

   // Tell the server the game has finished
   void gameEnded(string[] gameInfo)
   {

   }

   // Recieve message telling if character choice was successful
   void characterResult()
   {

   }

   // Assign a character pick to the correct player
   // character, playerIndex
   public void assignCharacter(string gameInfo)
   {
      string[] parameters = gameInfo.Split(Convert.ToChar(Constants.gameDivider));

      int index     = Int32.Parse(parameters[1]);
      int character = Int32.Parse(parameters[0]);

      lobbyPlayers[index].Character = character;
      characterScript.playerChoiceImages[index].sprite = Resources.Load<Sprite>(Characters.Names[character]) as Sprite;
      characterScript.currentPicker += 1;
      characterSelect.selectedCharacters[index] = character;
   }

   // Get the result of a dice roll
   void diceRoll(string[] gameInfo)
   {

   }

   void endTurn(string[] gamInfo)
   {

   }

   void startTurn(string[] gamInfo)
   {

   }

   void sendChat(string[] gamInfo)
   {

   }

   // Called if invalid network message is sent
   void networkError(string gamInfo)
   {

   }

   // Network commands called by external scripts
 #region Game Commands
   
   // Send character select choice
   // character, playerIndex
   public void sendCharacterSelect(int character, int playerIndex)
   {
      string message = Constants.characterSelect + Constants.commandDivider + character + Constants.gameDivider + playerIndex;
      if (!isHostingGame)
         sendSocketMessage(message, hostConnectionID);
      else
         sendActionToClients(message, 0);
   }

   // Send the two numbers that make up the dice roll
   // Number1, Number2
   public void sendDiceRoll(int number1, int number2, int targetID = hostConnectionID)
   {
      string message = Constants.diceRoll + Constants.commandDivider + number1 + Constants.gameDivider + number2;
      if (!isHostingGame)
         sendSocketMessage(message, hostConnectionID);
      else
         sendActionToClients(message, 0);
   }

   // Sends two coordinates of settlement
   public void sendBuildSettlement(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.buildSettlement + Constants.commandDivider + x + Constants.gameDivider + y;
      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendUpgradeToCity(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.upgradeToCity + Constants.commandDivider + x + Constants.gameDivider + y;  
      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendBuildRoad(int ax, int ay, int bx, int by, int targetID = hostConnectionID)
   {
      string message = Constants.buildRoad + Constants.commandDivider + ax + Constants.gameDivider + ay
                                              + Constants.gameDivider + bx + Constants.gameDivider + by;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendBuildArmy(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.buildArmy + Constants.commandDivider + x + Constants.gameDivider + y;
      // May need to send a random int as Unity might think they are the same message

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendAttackCity(int attackerX, int attackerY, int defenderX, int defenderY, int targetID = hostConnectionID)
   {
      string message = Constants.attackCity + Constants.commandDivider + attackerX + Constants.gameDivider + attackerY
                                               + Constants.gameDivider + defenderX + Constants.gameDivider + defenderY;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendMoveRobber(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.moveRobber + Constants.commandDivider + x + Constants.gameDivider + y;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendEndTurn(int targetID = hostConnectionID)
   {
      string message = Constants.endTurn + Constants.commandDivider;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendStartTurn(int targetID = hostConnectionID)
   {
      String message = Constants.startTurn + Constants.commandDivider;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendSendChat(int chatMessageNumber, int targetID = hostConnectionID)
   {
      string message = Constants.sendChat + Constants.commandDivider + chatMessageNumber;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

#endregion

   // Process ingame Commands from the network
   public void gameCommandProcessing(string commandCode, string commandInfo, int connectionID)
   {
      string[] gameInfo = commandInfo.Split(Convert.ToChar(Constants.gameDivider));

      switch (commandCode)
      {
         case Constants.characterSelect:
            assignCharacter(commandInfo);
            break;
         case Constants.diceRoll:
            // Number 1, Number2
            mapObject.DiceRollNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]));
            break;
         case Constants.buildSettlement:
            // X = gameInfo[0], Y = gameInfo[1]
			   mapObject.BuildSettlementNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]));
            break;
         case Constants.upgradeToCity:
            // X = gameInfo[0], Y = gameInfo[1]
			   mapObject.BuildCityNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]));
            break;
         case Constants.buildRoad:
            // AX = gameInfo[0], AY = gameInfo[1], BX = gameInfo[2], BY = gameInfo[3]
			   mapObject.BuildRoadNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]), Int32.Parse(gameInfo[2]), Int32.Parse(gameInfo[3]));
            break;
         case Constants.buildArmy:
            // X = gameInfo[0], Y = gameInfo[1]
			   mapObject.BuyArmyNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]));
            break;
         case Constants.attackCity:
            // X = gameInfo[0], Y = gameInfo[1]
			   mapObject.ExecuteAttackNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]), Int32.Parse(gameInfo[2]), Int32.Parse(gameInfo[3]));
            break;
         case Constants.moveRobber:
            // X = gameInfo[0], Y = gameInfo[1]
			   mapObject.MoveRobberNetwork(Int32.Parse(gameInfo[0]), Int32.Parse(gameInfo[1]));
            break;
         case Constants.endTurn:
            // No parameters
            mapObject.NextPlayer();
            break;
         case Constants.startTurn:
            // No parameters
            break;
         case Constants.sendChat:
            // Message number = gameInfo[0]
            break;
      }

      if (isHostingGame)
      {
         sendActionToClients(commandCode + Constants.commandDivider + commandInfo, connectionID);
      }
   }

   // Distributes a network message to all clients
   void sendActionToClients(string message, int connectionID)
   {
      foreach (Player player in lobbyPlayers)
      {
         if (player.connectionID != connectionID)
         {
            sendSocketMessage(message, player.connectionID);
            Debug.Log("Send action to " + player.connectionID + "  " + message);
         }
      }
   }
}