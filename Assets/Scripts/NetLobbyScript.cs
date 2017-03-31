
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetLobbyScript : MonoBehaviour {
    
    [System.Serializable]
    public struct netLobbyText
    {
        public Text gameNameText;
        public Text hostNameText;
        public Text playerNumberText;
        public Text statusText;
    }
    
    public struct netLobbyValues
    {  
        public int    numOfPlayers;
        public string hostName;  
        public string gameName;  
        public string status;
    }

    public IList<netLobbyText> netLobbyTextList = new List<netLobbyText>();
    public IList<netLobbyValues> netLobbyValuesList = new List<netLobbyValues>();

    public void addNetLobbyValues()
    {
        netLobbyValuesList.Add(new netLobbyValues(){numOfPlayers = 6, hostName = "Ghost", gameName = "The Ender", status = "started"});
    }

    public void returnToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
	
	public void PreGameBoardSceneOn()
    {
        BoardManager.startingGame = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BoardManager");
    }
}
