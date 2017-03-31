using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobbyScript : MonoBehaviour {

   public IList<string> playerNameList = new List<string>();
   
   public void addPlayers()
   {
       playerNameList.Add("John Rango");
   }

   public void returnToMain()
   {
	  UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
   }
   public void netLobbyOn()
   {
	  UnityEngine.SceneManagement.SceneManager.LoadScene("Network Lobby");
   }
      public void startGame()
   {
	  UnityEngine.SceneManagement.SceneManager.LoadScene("Character Select");
   }  
}