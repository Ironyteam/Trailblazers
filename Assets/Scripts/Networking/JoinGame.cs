using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour
{ 

   public Button joinBTN;
   public NetworkManager networkThing;

   // Use this for initialization
   void Start ()
   {
      joinBTN.onClick.AddListener(() => StartCoroutine(joinGame()));
      joinBTN.transform.GetComponentInChildren<Text>().text = "Join";
   }
        
   IEnumerator joinGame()
   {
      if (NetworkManager.inPlayerLobby == false)
      {
         Text[] gameTextBoxes = joinBTN.transform.parent.transform.GetComponentsInChildren<Text>();
         networkThing = GameObject.Find("Network Handler").GetComponent<NetworkManager>();
         networkThing.myGame.mapName = gameTextBoxes[4].text;
         NetworkManager.inPlayerLobby = true;
         int hostId = networkThing.connectToHost(gameTextBoxes[6].text);
         networkThing.onJoinGameClient();
         yield return new WaitForSecondsRealtime(1);
         networkThing.requestGameJoin(hostId);
         Destroy(gameObject);
<<<<<<< HEAD
=======
         networkThing.clearGamePanel();
>>>>>>> master
      }
   }
}
