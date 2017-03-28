using System;
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
   const int serverConnectionID = 1; // Should always be zero but may need to reconfigure
   const int hostConnectionID = 2; // Should be two may need to change
   const string myIP = "127.0.0.1";
   public List<Player> lobbyPlayers = new List<Player>();
   Player myPlayer;

   bool isHostingGame = false;
   bool inGameLobby   = false;
   bool inGame        = false;

   List<string[]> gameList = new List<string[]>();
   public GameObject gameInfoPanel;
   public GameObject gameListCanvas;
   public GameObject playerInfoPanel;
   NetworkGame myGame;
   public GameBoard mapObject; // Used to find the GameObject

   public Text messageLog;
   public Text ipField;

   // Buttons for Network Lobby scene
   public Button connectServerBTN;
   public Button hostGameBTN;
   public Button refreshGameListBTN;
   public Button cancelHostingBTN;
   public Button startGameBTN;

   // Game settings input fields
   public Text gameName;
   public Text maxPlayers;
   public Text gamePassword;
   public Text mapName;

   public string MapName = "DEFAULTMAP";

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
      requestGameList("172.16.51.127~Name~4~5~passwod~map");

      myPlayer = new Player("DefaultName");

      // Hook up all buttons if in lobby screen

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
         gameName = GameObject.Find("gameTXT").GetComponent<UnityEngine.UI.Text>();
         maxPlayers = GameObject.Find("maxPlayersTXT").GetComponent<UnityEngine.UI.Text>();
         gamePassword = GameObject.Find("passwordTXT").GetComponent<UnityEngine.UI.Text>();
         mapName = GameObject.Find("mapTXT").GetComponent<UnityEngine.UI.Text>();

         // Buttons linkup
         connectServerBTN = GameObject.Find("ServerBTN").GetComponent<Button>();
         connectServerBTN.onClick.AddListener(() => connectToServerBTN());
         hostGameBTN = GameObject.Find("HostGameBTN").GetComponent<Button>();
         hostGameBTN.onClick.AddListener(() => hostGame());
         refreshGameListBTN = GameObject.Find("RefreshBTN").GetComponent<Button>();
         refreshGameListBTN.onClick.AddListener(() => refreshGameList());
         cancelHostingBTN = GameObject.Find("CancelHostingBTN").GetComponent<Button>();
         cancelHostingBTN.onClick.AddListener(() => cancelGame());
         startGameBTN = GameObject.Find("StartGameBTN").GetComponent<Button>();
         if (!isHostingGame)
            startGameBTN.gameObject.SetActive(false);
         startGameBTN.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("Character Select"));

         GameObject.Find("Network Handler").GetComponent<NetworkManager>();
         Debug.Log("Loaded Network Lobby");
      }
   }

   #region Temp Functions
   // Function to name player for button TEST
   public void namePlayer()
   {
      // Will eventually be what you name is
   }
   // Function to allow a testing button press, as inpector doesn't allow button function calls that have parameters TEST
   public void connectToServerBTN()
   {
      connectToGame(ipField.text);
   }
#endregion

   // Connect to ipAddress, target auto connects in return
   public void connectToGame(string ipAddress)
   {
      byte error;
      Debug.Log("\n" + "Trying to connect to: " + ipAddress);
      connectionId = NetworkTransport.Connect(0, ipAddress, socketPort, 0, out error);
      Debug.Log("\n" + "ConnectionID: " + connectionId);
   }

   // Send game info to the server
   public void hostGame()
   {
      if (!isHostingGame)
      {
         string gameInfo;
         isHostingGame = true;
         startGameBTN.gameObject.SetActive(true);
         // Initialize the network game for hosting
         myGame = new NetworkGame()
         {
            gameName = gameName.text,
            numberOfPlayers = "0",
            maxPlayers = maxPlayers.text,
            password = gamePassword.text,
            mapName = MapName,
         };

         gameInfo = Constants.addGame + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myGame.gameName +
            Constants.gameDivider + "0" + Constants.gameDivider + myGame.maxPlayers + Constants.gameDivider + myGame.password + Constants.gameDivider + myGame.mapName;
         sendSocketMessage(gameInfo, serverConnectionID);
      }
      else
      {
         Debug.Log("\nError: Already hosting a game.");
      }
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
         case Constants.requestGameList:   // #, game, game, game, game...
            requestGameList(gameInfo[1]);
            break;
         case Constants.cancelGame:        // #
            removeGame(gameInfo[1]);
            break;
         case Constants.gameStarted:       // #, ipAddress
         case Constants.gameEnded:         // #, ipAddress
         case Constants.characterSelect:   // #, character
            break;
         case Constants.characterResult:   // #, characterResult
            break;
         case Constants.diceRoll:          // #, number1, number2
            break;
         case Constants.networkError:      // #, info
            networkError(gameInfo[1]);
            break;
         case Constants.buildSettlement:
         case Constants.upgradeToCity:
         case Constants.buildRoad:
         case Constants.buildArmy:
         case Constants.attackCity:
         case Constants.moveRobber:
         case Constants.endTurn:
         case Constants.startTurn:
         case Constants.sendChat:
            gameCommandProcessing(gameInfo[0], gameInfo[1], recConnectionID);
            break;
         default:
            networkError(gameInfo[1]);
            break;
      }
   }

   // Send request to host to join game
   public void requestGameJoin()
   {
      Debug.Log("\nRequesting to join game, connectionID:" + connectionId);
      string message = Constants.addPlayer + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myPlayer.Name;
      sendSocketMessage(message, connectionId);
   }

   // Player connecting to your lobby
   public void addPlayer(string gameInfo, int connectionID)
   {
      Debug.Log("\nInside add player");
      string[] playerInfo = gameInfo.Split(Convert.ToChar(Constants.gameDivider));

      // Destroy everything in the panel
      foreach (Transform child in gameListCanvas.transform)
      {
         GameObject.Destroy(child.gameObject);
      }

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
         ipAddress = gameInfo
      };

      lobbyPlayers.Add(newplayer);

      // Create UI GameObject to list player
      GameObject newPlayer = Instantiate(playerInfoPanel, gameListCanvas.transform, false);
      Text[] playerTexts = newPlayer.GetComponentsInChildren<Text>();
      playerTexts[0].text = myGame.numberOfPlayers;
      playerTexts[1].text = playerInfo[1];
      playerTexts[3].text = playerInfo[0];
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

   // Send character select choice
   void characterSelect(string[] gameInfo)
   {

   }

   // Recieve message telling if character choice was successful
   void characterResult(string[] gameInfo)
   {

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

   // Network commands exclusive to gameplay
#region Game Commands

   // Sends two coordinates of settlement
   public void sendBuildSettlement(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.buildSettlement + Constants.commandDivider + x + Constants.gameDivider + y;
      if (!isHostingGame)
      {
         sendSocketMessage(message, targetID);
      }
      else
      {
         sendActionToClients(message, 0);
      }
   }

   public void sendUpgradeToCity(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.upgradeToCity + Constants.commandDivider + x + Constants.gameDivider + y;

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendBuildRoad(int ax, int ay, int bx, int by, int targetID = hostConnectionID)
   {
      string message = Constants.buildRoad + Constants.commandDivider + ax + Constants.gameDivider + ay
                                              + Constants.gameDivider + bx + Constants.gameDivider + by;

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendBuildArmy(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.buildArmy + Constants.commandDivider + x + Constants.gameDivider + y;
      // May need to send a random int as Unity might think they are the same message

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendAttackCity(int attackerX, int attackerY, int defenderX, int defenderY, int targetID = hostConnectionID)
   {
      string message = Constants.attackCity + Constants.commandDivider + attackerX + Constants.gameDivider + attackerY
                                               + Constants.gameDivider + defenderX + Constants.gameDivider + defenderY;

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendMoveRobber(int x, int y, int targetID = hostConnectionID)
   {
      string message = Constants.moveRobber + Constants.commandDivider + x + Constants.gameDivider + y;

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendEndTurn(int targetID = hostConnectionID)
   {
      string message = Constants.endTurn + Constants.commandDivider;

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendStartTurn(int targetID = hostConnectionID)
   {
      String message = Constants.startTurn + Constants.commandDivider;

      if (isHostingGame)
         sendSocketMessage(message, targetID);
      else
         sendActionToClients(message, 0);
   }

   public void sendSendChat(int chatMessageNumber, int targetID = hostConnectionID)
   {
      string message = Constants.sendChat + Constants.commandDivider + chatMessageNumber;

      if (isHostingGame)
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
            mapObject.EndTurnNetwork();
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

   void sendActionToClients(string message, int connectionID)
   {
      foreach (Player player in lobbyPlayers)
      {
         if (player.connectionID != connectionID)
            sendSocketMessage(message, player.connectionID);
      }
   }
}