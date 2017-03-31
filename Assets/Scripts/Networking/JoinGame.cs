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
   }
        
   IEnumerator joinGame()
   {
      Text[] gameTextBoxes = joinBTN.transform.parent.transform.GetComponentsInChildren<Text>();
      Debug.Log(gameTextBoxes.Length);
      Debug.Log(gameTextBoxes[6].text);
      networkThing = GameObject.Find("Network Handler").GetComponent<NetworkManager>();
      networkThing.connectToGame(gameTextBoxes[6].text);
      yield return new WaitForSecondsRealtime(2);
      networkThing.requestGameJoin();
   }
}
