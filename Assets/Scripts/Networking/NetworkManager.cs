﻿using System;
using System.Collections;
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
   int serverConnectionID        = 1; // Updates when you connect to a server
   public int hostConnectionID   = -1;
   const string myIP = "127.0.0.1";
   public string serverIP = "192.168.0.2";
   public int loadedPlayers = 0;
   public List<Player> lobbyPlayers = new List<Player>();
   public Player myPlayer;
   GameObject newPlayerPNL;
   Player player;

   bool isHostingGame                   = false;
   public static bool inPlayerLobby     = false;
   public static bool inCharacterSelect = false;
   public static bool inGame            = false;

   List<string[]> gameList = new List<string[]>();
   public GameObject gameInfoPanel;
   public GameObject gameListCanvas;
   public GameObject playerInfoPanel;
   public NetworkGame myGame;
   public GameBoard mapObject; // Used to find Luke's game script GameObject
   public Text MessagePopup; // Used to show message to user

   public Text messageLog;
   public Text ipField;

   // Buttons for scenes
   public Button connectServerBTN;
   public Button hostGameBTN;
   public Button returnToMenuBTN;
   public Button refreshGameListBTN;
   public Button cancelHostingBTN;
   public Button startGameBTN;
   public Button createGameBTN;
   public Button startGameCharacterSelectBTN;
   public Button quitToMenu;
   public Button quitToMenuEnd;
   public characterSelect characterScript;
   public loadgameSpin    loadGameSpin;

   // Game settings input fields
   public Text gameName;
   public Text maxPlayers;
   public Text gamePassword;
   public Text mapName;

   public string gameNameTemp        = "Default";
   public string numberOfPlayersTemp = "0";
   public string maxPlayersTemp      = "0";
   public string passwordTemp        = "Default";
   public string mapNameTemp         = "Default";

#endregion

#region StartFunctions
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
      int maxConnections = 20;

      // Create Saved Maps directory if one does not exist
      if (!Directory.Exists(Application.dataPath + "/Config"))
         Directory.CreateDirectory(Application.dataPath + "/Config");
      // Create ip config file if one does not exist
      if (!File.Exists(Application.dataPath + "/Config/config.cfg"))
      {
         using (StreamWriter temp = File.CreateText(Application.dataPath + "/Config/config.cfg"))
         {
            temp.Write(serverIP);
            temp.Close();
         }
      }
      // Read the server ip address into the variable
      using (StreamReader reader = File.OpenText(Application.dataPath + "/Config/config.cfg"))
      {
         string tempString;

         while ((tempString = reader.ReadLine()) != null)
         {
            serverIP = tempString;
         }
         reader.Close();
      }

      // Opens a port on local computer that will be used for sending and recieving, done on client and server
      NetworkTransport.Init();
      ConnectionConfig config = new ConnectionConfig();
      myReliableChannelId = config.AddChannel(QosType.Reliable);
      HostTopology topology = new HostTopology(config, maxConnections);
      socketId = NetworkTransport.AddHost(topology, socketPort);

      // Create an fake game on launch for testing TEST
      //requestGameList("172.16.51.124~Refresh Game~4~5~password~List;172.16.51.127~To See2~4~5~password~Network Games");
      myGame = new NetworkGame();
      myPlayer = new Player("Silas");
      myPlayer.connectionID = 0;
      connectToServer(serverIP);
      StartCoroutine(startRefreshGameList());
    }
   
   IEnumerator startRefreshGameList()
    {
        yield return new WaitForSeconds(1);
        requestGameListServer();
    }
   private void OnEnable()
   {
      SceneManager.sceneLoaded += hookUpLobbyFunctionality;
   }

   private void OnDisable()
   {
      SceneManager.sceneLoaded -= hookUpLobbyFunctionality;
   }

   // Hook up external buttons and objects depending on scene loaded, called every scene change
   private void hookUpLobbyFunctionality(Scene scene, LoadSceneMode mode)
   {
        if (scene.name == "Network Lobby" && NavigationScript.networkGame)
        {
            // UI linkups, panels and fields
            gameInfoPanel = Resources.Load("GameInfoPNL") as GameObject;
            gameListCanvas = GameObject.Find("ContentPNL");
            playerInfoPanel = Resources.Load("PlayerInfoPNL") as GameObject;
            messageLog = GameObject.Find("MessageLogTXT").GetComponent<Text>();
            ipField = GameObject.Find("ipTXT").GetComponent<Text>();
            MessagePopup = GameObject.Find("NotificationMessageTXT").GetComponent<Text>();
            MessagePopup.gameObject.SetActive(false);

            // Buttons linkup
            connectServerBTN = GameObject.Find("ServerBTN").GetComponent<Button>();
            connectServerBTN.onClick.AddListener(() => connectToServer(serverIP));
            refreshGameListBTN = GameObject.Find("RefreshBTN").GetComponent<Button>();
            refreshGameListBTN.onClick.AddListener(() => requestGameListServer());
            returnToMenuBTN = GameObject.Find("Main Menu").GetComponent<Button>();
            returnToMenuBTN.onClick.AddListener(() => userQuitToMenu());
            cancelHostingBTN = GameObject.Find("CancelHostingBTN").GetComponent<Button>();
            cancelHostingBTN.onClick.AddListener(() => cancelGame());
            createGameBTN = GameObject.Find("CreateGameBTN").GetComponent<Button>();
            createGameBTN.onClick.AddListener(() => createGame());
            startGameBTN = GameObject.Find("StartGameBTN").GetComponent<Button>();
            startGameBTN.onClick.AddListener(() => startCharacterSelect());
            startGameBTN.gameObject.SetActive(false);
            ipField.gameObject.SetActive(false);
            ipField.transform.parent.gameObject.SetActive(false);
            connectServerBTN.gameObject.SetActive(false);
            if (!isHostingGame)
            {
                startGameBTN.gameObject.SetActive(false);
                cancelHostingBTN.gameObject.SetActive(false);
            }
            else
            {
                // Destroy the list of network games in the panel
                clearGamePanel();
                lobbyPlayerChange(1); // Add myself to the lobby
                //startGameBTN.gameObject.SetActive(true);
                createGameBTN.gameObject.SetActive(false);
                refreshGameListBTN.gameObject.SetActive(false);
                connectServerBTN.gameObject.SetActive(false);
            }
        }
        else if (scene.name == "Character Select" && NavigationScript.networkGame)
        {
            startGameCharacterSelectBTN = GameObject.Find("Start Game").GetComponent<Button>();
            startGameCharacterSelectBTN.onClick.AddListener(() => characterSelectStartGame());
            characterScript = GameObject.Find("Main Camera").GetComponent<characterSelect>();
            loadGameSpin = GameObject.Find("loadingScript").GetComponent<loadgameSpin>();
            if (!isHostingGame)
                startGameCharacterSelectBTN.enabled = false;
        }
        else if (scene.name == "In Game Scene" && NavigationScript.networkGame)
        {
            quitToMenu = GameObject.Find("Quit to Menu").GetComponent<Button>();
            quitToMenu.onClick.AddListener(() => userQuitToMenu());
            quitToMenuEnd = GameObject.Find("Quit to Menu Scoreboard").GetComponent<Button>();
            quitToMenuEnd.onClick.AddListener(() => userQuitToMenu());
            mapObject = GameObject.Find("Map").GetComponent<GameBoard>();
            mapObject.LocalGame.isNetwork = true;
        }
        else if (scene.name == "Menu" && NavigationScript.networkGame)
        {
           isHostingGame = false;
           cleanUpNetworking();
        }
   }
#endregion

   // Called every frame, checks for incoming network messages
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
            Debug.Log("\n" + "Incoming connection: ");
            Debug.Log("recHostId = " + recHostId + "recConnectionId = " + recConnectionId);
            break;
         case NetworkEventType.DataEvent:
            Stream stream = new MemoryStream(recBuffer);
            BinaryFormatter formatter = new BinaryFormatter();
            string message = formatter.Deserialize(stream) as string;
            Debug.Log("\nIncoming Data: " + message);
            processNetworkMessage(message, recConnectionId);
            break;
         case NetworkEventType.DisconnectEvent:
            Debug.Log("\n" + "Remote client disconnected");
            player = lobbyPlayers.Find(item => item.connectionID == recConnectionId);
            if (player != null && isHostingGame && inGame && !inCharacterSelect) // Player left during game
            {
               Debug.Log("Player disconnected this should be host");
               playerDisconnected(player);
            }
            else if (lobbyPlayers.Count > 0 && recConnectionId == hostConnectionID && inGame) // Host disconnected close game and reset
            {
               Debug.Log("Host disconnected");
               hostDisconnected();
               lobbyPlayers.Clear();
            }
            else if (recConnectionId == serverConnectionID) // Server disconnected keep trying to reconnect will be a reconnect attempt every 4-8 seconds
            {
               Debug.Log("Server disconnected");
               // Server disconnected
               connectToServer(serverIP);
            }
            else if (inCharacterSelect && isHostingGame) // Player left during character select cancel game
            {
               Debug.Log("Player left during character select");
               endNetworkGame(player);
            }
            else if (inPlayerLobby && isHostingGame) // Player in the lobby left
            {
               Debug.Log("Player left the lobby");
               lobbyPlayers.Remove(player);
               lobbyPlayerChange(lobbyPlayers.Count);
               sendActionToClients(Constants.playerAdded + Constants.commandDivider + lobbyPlayers.Count, -1);
               NetworkTransport.Disconnect(recHostId, recConnectionId, out error);
            }
            else if (inPlayerLobby)
            {
               Debug.Log("Join Game Full");
               joinGameFull();
            }
            else
            {
               Debug.Log("Player disconnected randomly: inCharacterSelect = " + inCharacterSelect + " inGame = " + inGame + " inPlayerLobby = " + inPlayerLobby);
            }
            break;
      }
   }

   public void processNetworkMessage(string networkMessage, int recConnectionID)
   {
      string[] gameInfo = networkMessage.Split(Convert.ToChar(Constants.commandDivider));

      switch (gameInfo[0])
      {
         case Constants.addPlayer:         // #, ipAddress, password
            // Player requesting to join game
            addPlayer(gameInfo[1], recConnectionID);
            break;
         case Constants.lobbyFull:
            joinGameFull();
            break;
         case Constants.playerDisconnect:
            clientPlayerDisconnected(Int32.Parse(gameInfo[1]));
            break;
         case Constants.playerAdded:
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
            inGame = true;
            inPlayerLobby = false;
            inCharacterSelect = true;
            BoardManager.template = myGame.gameMap;
            SceneManager.LoadScene("Character Select");
            break;
         case Constants.characterResult:   // #, characterResult
            break;
         case Constants.gameStarted:
            startInGameLoad();
            break;
         case Constants.inGameSceneLoaded:
            clientInGameLoaded();
            break;
         case Constants.enterInGameScene:
            loadGameSpin.async.allowSceneActivation = true;
            break;
         case Constants.gameEnded:         // #, ipAddress
            // Game ended
            endNetworkGame(myPlayer); // Client is calling this so the player doesn't matter
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

   // Connect to the server
   public void connectToServer(string ip)
   {
      byte error;
      if (ipField.text != "" && ipField.text != null)
         serverIP = ipField.text;
      serverConnectionID = NetworkTransport.Connect(socketId, serverIP, socketPort, 0, out error);
      connectServerBTN.gameObject.SetActive(false);
      ipField.transform.parent.gameObject.SetActive(false);
      ipField.gameObject.SetActive(false);
      Debug.Log("\n Connecting to server: " + serverIP + "  Server ConnectionID: " + serverConnectionID);
   }

   // Connect to ipAddress, target auto connects in return
   public void connectToGame(string ipAddress)
   {
      byte error;
      hostConnectionID = NetworkTransport.Connect(socketId, ipAddress, socketPort, 0, out error);
      Debug.Log("\n" + "connectToGame assigned hostConnectionID = " + hostConnectionID);
   }

   // Connect to game test with it returning connection id
   public int connectToHost(string ipAddress)
   {
      byte error;
      hostConnectionID = NetworkTransport.Connect(socketId, ipAddress, socketPort, 0, out error);
      Debug.Log("\n" + "connectToGame assigned & returned hostConnectionID = " + hostConnectionID);
      return hostConnectionID;
   }


   // Create a empty game
   public void createGame()
   {
      isHostingGame = true;
   }

   // Set the values recieved from BoardManager and send the server the game
   public void setupGameSettings(int totalPlayers, int turnTimer, int victoryPoints, bool charAbilitiesOn, string gameName, string mapName, HexTemplate map)
   {
      myGame.maxPlayers = totalPlayers.ToString();
      myGame.turnTimer  = turnTimer.ToString();
      myGame.numberOfVictoryPoints = victoryPoints.ToString();
        if (charAbilitiesOn)
            myGame.abilitiesOn = "1";
        else
            myGame.abilitiesOn = "0";
      myGame.gameName   = gameName;
      myGame.mapName    = mapName;
      myGame.gameMap    = map;
      myGame.mapString  = map.ToFileString();
      // Splits the map into four pieces to make it small enought to send over the network
      myGame.mapPiece1  = myGame.mapString.Substring(0, 400);
      myGame.mapPiece2  = myGame.mapString.Substring(400, 400);
      myGame.mapPiece3  = myGame.mapString.Substring(800, 400);
      myGame.mapPiece4  = myGame.mapString.Substring(1200, myGame.mapString.Length - 1200);
      hostGame();
   }

   // Send game info to the server
   public void hostGame()
   {
      if (isHostingGame)
      {
         inPlayerLobby = true;
         string gameInfo;
         loadedPlayers = 0;
         myPlayer.connectionID = 0;
         myPlayer.playerIndex  = 0;
         lobbyPlayers.Add(myPlayer);
         gameInfo = Constants.addGame + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myGame.gameName +
         Constants.gameDivider + "0" + Constants.gameDivider + myGame.maxPlayers + Constants.gameDivider + myGame.password + Constants.gameDivider + myGame.mapName;
         sendSocketMessage(gameInfo, serverConnectionID);
      }
   }

   // Host goes to character select and tell the clients to go there, also tells the server game has started
   public void startCharacterSelect()
   {
      // Tell server the game has started
      sendSocketMessage(Constants.gameStarted + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);

      inCharacterSelect = true;
      inPlayerLobby = false;
      inGame = true; // At this point the game is started and if a player leaves his spot will stay empty
      BoardManager.template          = myGame.gameMap;
      BoardManager.numOfPlayers      = Int32.Parse(myGame.maxPlayers);
      BoardManager.localPlayerIndex  = myPlayer.playerIndex;
      sendActionToClients(Constants.goToCharacterSelect + Constants.commandDivider + Network.player.ipAddress, 0);
      sendPlayerNumbers();
      SceneManager.LoadScene("Character Select");
   }

   // User quit to menu from 


   // User quit to menu from in game
   public void userQuitToMenu()
   {
      byte error;

      Debug.Log("userQuitToMenu: called");
      if (isHostingGame)
      {
         Debug.Log("userQuitToMenu: host quit");
         endNetworkGame(myPlayer);
      }
      else
      {
         SceneManager.LoadScene("Menu");
         Debug.Log("userQuitToMenu: player quit");
         if (inPlayerLobby)
            NetworkTransport.Disconnect(myPlayer.connectionID, hostConnectionID, out error);
      }
      lobbyPlayers.Clear();
      inGame = false;
   }

   // End the game and disconnect all connections other than the server
   public void endGameNetwork()
   {
      byte error;
      inGame = false; // The game is over
      // Close all connections other than serverConnectionID
      for (int i = 1; i < lobbyPlayers.Count; i++)
      {
         NetworkTransport.Disconnect(myPlayer.connectionID, lobbyPlayers[i].connectionID, out error);
      }
   }

   // Send all players what index they are on the player list
   public void sendPlayerNumbers()
   {
      lobbyPlayers[0].playerIndex = 0;
      for (int i = 1; i < lobbyPlayers.Count; i++)
      {
         lobbyPlayers[i].playerIndex = i;
         sendSocketMessage(Constants.playerNumber + Constants.commandDivider + i, lobbyPlayers[i].connectionID);
      }
   }

   // Create a player list and add myself in the correct index position
   public void setupPlayersOnClient(int myIndex)
   {
      Debug.Log("Max players " + myGame.maxPlayers + " -  My index " + myIndex);
      for (int i = 0; i < Int32.Parse(myGame.maxPlayers); i++)
      {
         player = new Player();
         player.playerIndex = i;
         lobbyPlayers.Add(player);
      }
      myPlayer.playerIndex = myIndex;
      lobbyPlayers[myIndex] = myPlayer;
      BoardManager.localPlayerIndex = myPlayer.playerIndex;
      Debug.Log("BoardManager.locaplayerindex = " + BoardManager.localPlayerIndex);
   }

   // The number of lobby players changes
   public void lobbyPlayerChange(int numPlayers)
   {
      clearGamePanel();
      for (int i = 1; i <= numPlayers; i++)
      {
         GameObject newPlayerPNL = Instantiate(playerInfoPanel, gameListCanvas.transform, false);
         if (i == 1)
            newPlayerPNL.GetComponentInChildren<Text>().text = "     Player " + i + " (Host)";
         else
            newPlayerPNL.GetComponentInChildren<Text>().text = "     Player " + i;
      }

      if (isHostingGame)
      {
         if (lobbyPlayers.Count == Int32.Parse(myGame.maxPlayers))
            startGameBTN.gameObject.SetActive(true);
         else
            startGameBTN.gameObject.SetActive(false);
      }
   }

   // Clear the game info panel to display new games or new players
   public void clearGamePanel()
   {
      foreach (Transform child in gameListCanvas.transform)
      {
         GameObject.Destroy(child.gameObject);
      }
   }

   // The host left the game
   public void hostDisconnected()
   {
      SceneManager.LoadScene("Menu");
      onLeaveGameClient();
      inGame = false;
   }

   // Tell the clients to go to in game scene
   public void characterSelectStartGame()
   {
      inGame = true;
      inCharacterSelect = false;
      sendActionToClients(Constants.gameStarted + Constants.commandDivider + Network.player.ipAddress, 0);
   }

   // Start loading the in game scene
   public void startInGameLoad()
   {
      inCharacterSelect = false;
      Debug.Log("startInGameLoad: load started");
      Debug.Log("Load script name " + loadGameSpin.name);
      int count = 0;
      loadGameSpin.loadBoard();
   }

   // A player told host he finished loading the in game scene
   public void clientInGameLoaded()
   {
      loadedPlayers += 1;
      Debug.Log("clientInGameLoaded: Player In Game load finished");
      if (loadedPlayers >= lobbyPlayers.Count)
      {
         // Tell the clients to enter game
         string message = Constants.enterInGameScene + Constants.commandDivider + Network.player.ipAddress;
         sendActionToClients(message, -1);
         enterInGameScene();
      }
   }

   // Start the game by entering the in game scene
   public void enterInGameScene()
   {
      // Set the async.allowSceneActivation to enter in game scene
      loadGameSpin.async.allowSceneActivation = true;
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
      Debug.Log("\nSending message to ID " + connectionNum + " : " + message);
      NetworkTransport.Send(socketId, connectionNum, myReliableChannelId, buffer, bufferSize, out error);
   }

   // Show a message to the player for a brief time
   IEnumerator showLobbyMessage(string message)
   {
      MessagePopup.text = message;
      MessagePopup.gameObject.SetActive(true);
      yield return new WaitForSecondsRealtime(2f);
      MessagePopup.gameObject.SetActive(false);
   }

   // Send request to host to join game
   public void requestGameJoin(int hostId)
   {
      Debug.Log("\nrequestGameJoin: Connecting to game, hostConnectionID = " + hostConnectionID);
      string message = Constants.addPlayer + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myPlayer.Name;
      sendSocketMessage(message, hostId);
   }

   // Client disconnected during game set them as inactive so thier turn us skipped
   public void clientPlayerDisconnected(int playerIndex)
   {
      Debug.Log("clientPlayerDisconnected: setting player inactive");
      mapObject.LocalGame.PlayerList[playerIndex].isConnected = false;
   }

   // Player disconnected from the game
   public void playerDisconnected(Player player)
   {
      sendActionToClients(Constants.playerDisconnect + Constants.commandDivider + player.playerIndex, player.connectionID);
      mapObject.LocalGame.PlayerList[player.playerIndex].isConnected = false;
      Debug.Log("playerDisconnected: player.playerIndex = " + player.playerIndex + " mapObject.CurrentPlayer = " + mapObject.CurrentPlayer);
      if (player.playerIndex == mapObject.CurrentPlayer)
      {
         sendEndTurn(myPlayer.connectionID);
         mapObject.NextPlayer();
      }
   }

   // Joined game, disable buttons
   public void onJoinGameClient()
   {
      inPlayerLobby = true;

      createGameBTN.gameObject.SetActive(false);
      refreshGameListBTN.gameObject.SetActive(false);
      cancelHostingBTN.gameObject.SetActive(false);
   }

   // Left game, disable buttons
   public void onLeaveGameClient()
   {
      inPlayerLobby = false;
         
      //createGameBTN.gameObject.SetActive(true);
      //refreshGameListBTN.gameObject.SetActive(true);
      //cancelHostingBTN.gameObject.SetActive(true);
   }

   // The lobby requested to join is full go back to the in game lobby and request the game list
   public void joinGameFull()
   {
      inPlayerLobby = false;
      requestGameListServer();

      createGameBTN.gameObject.SetActive(true);
      refreshGameListBTN.gameObject.SetActive(true);
      cancelHostingBTN.gameObject.SetActive(false);

      StartCoroutine(showLobbyMessage("Failed to join game\nGame Full"));
   }

   // Player connecting to your lobby
   public void addPlayer(string gameInfo, int connectionID)
   {
      Debug.Log("\naddPlayer called");
      string[] playerInfo = gameInfo.Split(Convert.ToChar(Constants.gameDivider));


      // Tell connecting player he failed to to join lobby, either it is full or we are not hosting
      if (!isHostingGame || lobbyPlayers.Count >= Int32.Parse(myGame.maxPlayers) || inGame || inCharacterSelect)
      {
         Debug.Log("addPlayer: Lobby Join Failed - isHostingGame = " + isHostingGame + " lobbyPlayers.Count = " + lobbyPlayers.Count + " inGame = " + inGame + " inCharacterSelect = " + inCharacterSelect);
         sendlobbyFull(connectionID);
         return;
      }
      else // Add the player and tell the rest of the clients that
      {
         // Send the player the game info
         sendGameInfo(connectionID);

         // Send the player the map that is being played
         sendMap(myGame, connectionID);

         // Create player class
         Player newplayer = new Player()
         {
            connectionID = connectionID,
            ipAddress = gameInfo,
            playerIndex = lobbyPlayers.Count
         };

         lobbyPlayers.Add(newplayer);

         if (lobbyPlayers.Count == Int32.Parse(myGame.maxPlayers))
         {
            startGameBTN.gameObject.SetActive(true);
         }

         // Create UI GameObject to list player
         lobbyPlayerChange(lobbyPlayers.Count);

         // Send the player join event to all the players
         sendActionToClients(Constants.playerAdded + Constants.commandDivider + lobbyPlayers.Count, -1);
      }
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
   public void sendlobbyFull(int connectionID)
   {
      string message = Constants.lobbyFull + Constants.commandDivider + Network.player.ipAddress;
      sendSocketMessage(message, connectionID);
   }

   // Ask server to send list of games
   public void requestGameListServer()
   {
      StartCoroutine(showLobbyMessage("Refreshing Game List\nFrom Server"));
      foreach (NetworkPlayer playerConnection in Network.connections)
         Debug.Log("requestGameListServer: Open connections" + playerConnection.ipAddress);
      sendSocketMessage(Constants.requestGameList + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);
   }

   // Updates the local game list with list from server
   void requestGameList(string serverGameList)
   {
      gameList.Clear();
      string[] gameInfo = serverGameList.Split(Convert.ToChar(Constants.gameListDivider));
      Debug.Log("\n" + "requestGameList: Adding Games\n");
      foreach (string game in gameInfo)
      {
         string[] tempGame = game.Split(Convert.ToChar(Constants.gameDivider));
         gameList.Add(tempGame);
      }
      StartCoroutine(refreshGameList());
   }

   // Stop game from hosting if server sends a kill command
   public void removeGame(string gameInfo)
   {
      if (gameInfo == Constants.serverKillCode)
      {
         isHostingGame = false;
         Debug.Log("\nremoveGame: Your game has been canceled by the server");
         endNetworkGame(myPlayer);
      }
   }

   IEnumerator refreshGameList()
   {
      foreach (Transform child in gameListCanvas.transform)
      {
         GameObject.Destroy(child.gameObject);
      }
      if (gameList.Count == 0)
      {
          yield return new WaitForSeconds(1);
          StartCoroutine(showLobbyMessage("No Network Games Available"));
      }
      else
      {
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
      yield return null;
   }

   // Tell the server that game is canceled
   public void cancelGame()
   {
      if (isHostingGame)
      {
         isHostingGame = false;
         startGameBTN.gameObject.SetActive(false);
         lobbyPlayers.Clear();
         if (refreshGameListBTN != null)
         {
            refreshGameListBTN.gameObject.SetActive(true);
            createGameBTN.gameObject.SetActive(true);
            cancelHostingBTN.gameObject.SetActive(false);
         }
         sendSocketMessage(Constants.cancelGame + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);
      }
   }
   // Clean up networking after coming to menu
   public void cleanUpNetworking()
   {
        byte error;
        Debug.Log("endNetworkGame: canceling game");
        // Some of these should be false but just in case
        inGame = false;
        inCharacterSelect = false;
        inPlayerLobby = false;
        if (isHostingGame)
        {
            isHostingGame = false;
            sendActionToClients(Constants.gameEnded + Constants.commandDivider + 0, player.connectionID);
            sendSocketMessage(Constants.cancelGame + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);
            for (int i = 1; i < lobbyPlayers.Count; i++)
            {
                NetworkTransport.Disconnect(myPlayer.connectionID, lobbyPlayers[i].connectionID, out error);
            }
            lobbyPlayers.Clear();
        }
        else
        {
            Debug.Log("Trying to disconnect as client");
            NetworkTransport.Disconnect(myPlayer.connectionID, hostConnectionID, out error);
        }
        lobbyPlayers.Clear();
        myGame = new NetworkGame();
    }

   // Game ended clean up network connections and go to Menu scene
   public void endNetworkGame(Player player)
   {
      byte error;
      Debug.Log("endNetworkGame: canceling game");
      // Some of these should be false but just in case
      inGame = false;
      inCharacterSelect = false;
      inPlayerLobby = false;
      if (isHostingGame)
      {
         isHostingGame = false;
         sendActionToClients(Constants.gameEnded + Constants.commandDivider + 0, player.connectionID);
         sendSocketMessage(Constants.cancelGame + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);
         for (int i = 1; i < lobbyPlayers.Count; i++)
         {
            NetworkTransport.Disconnect(myPlayer.connectionID, lobbyPlayers[i].connectionID, out error);
         }
         SceneManager.LoadScene("Menu");
         lobbyPlayers.Clear();
      }
      else
      {
         Debug.Log("Trying to disconnect as client");
         SceneManager.LoadScene("Menu");
         NetworkTransport.Disconnect(myPlayer.connectionID, hostConnectionID, out error);
      }
      lobbyPlayers.Clear();
      myGame = new NetworkGame();
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
      Characters.PlayerChosen[character] = true;
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
   public void sendDiceRoll(int diceNumber, int targetID)
   {
      string message = Constants.diceRoll + Constants.commandDivider + diceNumber;
      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   // Sends two coordinates of settlement
   public void sendBuildSettlement(int x, int y, int targetID)
   {
      string message = Constants.buildSettlement + Constants.commandDivider + x + Constants.gameDivider + y;
      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendUpgradeToCity(int x, int y, int targetID)
   {
      string message = Constants.upgradeToCity + Constants.commandDivider + x + Constants.gameDivider + y;  
      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendBuildRoad(int ax, int ay, int bx, int by, int targetID)
   {
      string message = Constants.buildRoad + Constants.commandDivider + ax + Constants.gameDivider + ay
                                              + Constants.gameDivider + bx + Constants.gameDivider + by;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendBuildArmy(int x, int y, int targetID)
   {
      string message = Constants.buildArmy + Constants.commandDivider + x + Constants.gameDivider + y;
      // May need to send a random int as Unity might think they are the same message

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendAttackCity(int attackerX, int attackerY, int defenderX, int defenderY, int targetID)
   {
      string message = Constants.attackCity + Constants.commandDivider + attackerX + Constants.gameDivider + attackerY
                                               + Constants.gameDivider + defenderX + Constants.gameDivider + defenderY;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendMoveRobber(int x, int y, int targetID)
   {
      string message = Constants.moveRobber + Constants.commandDivider + x + Constants.gameDivider + y;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendEndTurn(int targetID)
   {
      string message = Constants.endTurn + Constants.commandDivider;

      if (!isHostingGame)
      {
         Debug.Log("sendEndTurn: Sending end turn as Client");
         sendSocketMessage(message, targetID);
      }
      else
      {
         Debug.Log("sendEndTurn: Sending end turn as Host");
         sendActionToClients(message, 0);
      }
   }

   public void sendStartTurn(int targetID)
   {
      String message = Constants.startTurn + Constants.commandDivider;

      if (!isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendSendChat(int chatMessageNumber, int targetID)
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
            mapObject.ReceiveDiceRoll(Int32.Parse(gameInfo[0]));
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
            Debug.Log("gameCommandProcessing: End turn constant recieved Player index = " + myPlayer.playerIndex + " hostConnectionID = " + hostConnectionID);
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
   void sendActionToClients(string message, int ignoredID)
   {
      if (isHostingGame)
      {
         foreach (Player player in lobbyPlayers)
         {
            if (player.connectionID != ignoredID && player.connectionID != myPlayer.connectionID && player.connectionID != serverConnectionID)
            {
               sendSocketMessage(message, player.connectionID);
               Debug.Log("sendActionToClients: Sending to ID" + player.connectionID + " Player index = " + player.playerIndex + "  :" + message);
            }
         }
      }
   }
}
