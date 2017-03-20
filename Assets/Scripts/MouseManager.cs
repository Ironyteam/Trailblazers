using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class MouseManager : MonoBehaviour {

    public AudioClip[] audioClip;
    public gameUIScriptLocal gameScriptLink;
	
	// Update is called once per frame
	void Update () {

      //  Debug.Log("Mouse Position: " + Input.mousePosition);

        // this only works in orthographic view
        // Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log("World Point: " + worldPoint);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Physics.Raycast (ray, out hitInfo)) {
				GameObject ourHitObject = hitInfo.collider.transform.gameObject;

				//Debug.Log("Raycast hit: " + ourHitObject.name);


				if (Input.GetMouseButtonDown (0)) {
					MeshRenderer mr = ourHitObject.GetComponentInChildren<MeshRenderer> ();
                    GameBoard currentGameBoard = GameObject.Find("Map").GetComponent<GameBoard>();
                    PlaySound (4);
                    mr.material = currentGameBoard.GetPlayerMaterial(currentGameBoard.CurrentPlayer);

                    if (ourHitObject.GetType() == currentGameBoard.Structures[0].Structure_GO.GetType())
                    {
                        foreach (Structure currentStructure in currentGameBoard.Structures)
                        {
                            if (currentStructure.Structure_GO == ourHitObject)
                            {
                                currentStructure.PlayerOwner = currentGameBoard.CurrentPlayer;
                                //currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
                                Debug.Log(currentStructure.Location.X + ", " + currentStructure.Location.Y);
                                currentGameBoard.HideAvailableSettlements();
                                if (currentGameBoard.InitialPlacement)
                                    currentGameBoard.ShowAvailableRoadsInitial(currentStructure.Location);
                                break;
                            }
                        }
                    }
                    if (ourHitObject.GetType() == currentGameBoard.Roads[0].Road_GO.GetType())
                    {
                        foreach (Road currentRoad in currentGameBoard.Roads)
                        {
                            if (currentRoad.Road_GO == ourHitObject)
                            {
                                currentRoad.PlayerOwner = currentGameBoard.CurrentPlayer;
                                //currentRoad.Road_GO.GetComponent<Collider>().enabled = false;
                                Debug.Log("Side A: " + currentRoad.SideA.X + ", " + currentRoad.SideA.Y);
                                Debug.Log("Side B: " + currentRoad.SideB.X + ", " + currentRoad.SideB.Y);
                                currentGameBoard.HideAvailableRoads();
                                break;
                            }
                        }
                    }
                }
			}
		}
	}

    void PlaySound(int clip)
    {
        GetComponent<AudioSource>().clip = audioClip[clip];
        GetComponent<AudioSource>().Play();
    }
}
