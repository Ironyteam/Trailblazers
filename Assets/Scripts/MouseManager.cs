using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class MouseManager : MonoBehaviour {

	public GameBoard currentGameBoard;
	public NetworkManager NetManager;

	private void Start()
	{
		currentGameBoard = GameObject.Find("Map").GetComponent<GameBoard>();
      if (currentGameBoard.LocalGame.isNetwork)
      {
         NetManager = GameObject.Find("Network Handler").GetComponent<NetworkManager>();
      }
		
	}

	// Update is called once per frame
	private void Update ()
	{
		// this only works in orthographic view
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Physics.Raycast (ray, out hitInfo))
			{
				GameObject ourHitObject = hitInfo.collider.transform.gameObject;

				if (Input.GetMouseButtonDown (0))
				{
					if (ourHitObject.GetType() == currentGameBoard.Structures[0].Structure_GO.GetType())
					{
						foreach (Structure currentStructure in currentGameBoard.Structures)
						{
							if (currentStructure.Structure_GO == ourHitObject)
							{
								// Current clicked structure is a settlement
								if (!currentStructure.IsCity)
								{
									// Current clicked settlement has no owner
									if (currentStructure.PlayerOwner == -1)
									{
										currentGameBoard.BuildSettlement(currentStructure);
										currentGameBoard.HideAvailableSettlements ();

										if (currentGameBoard.LocalGame.isNetwork)
                              {
                                 Debug.Log(currentStructure.Location.X + currentStructure.Location.Y);
											NetManager.sendBuildSettlement(currentStructure.Location.X, currentStructure.Location.Y, NetManager.hostConnectionID);
                              }
                              // If initial placement, show first road to build
                              if (currentGameBoard.InitialPlacement)
                              {
                                 currentGameBoard.ShowAvailableRoadsInitial(currentStructure.Location);
                                 currentGameBoard.LocalGame.PlayerList[currentGameBoard.CurrentPlayer].Settlements++;
                                 currentGameBoard.LocalGame.PlayerList[currentGameBoard.CurrentPlayer].UpdateVictoryPoints();
                              }
									}
									// Current clicked settlement is owned by current player
									else if (currentStructure.PlayerOwner == currentGameBoard.CurrentPlayer)
									{
										currentGameBoard.BuildCity(currentStructure);
										currentGameBoard.HideAvailableSettlementsToUpgrade();

										if (currentGameBoard.LocalGame.isNetwork)
											NetManager.sendUpgradeToCity(currentStructure.Location.X, currentStructure.Location.Y, NetManager.hostConnectionID);
									}
									break;
								}
								// Current clicked structure is a city 
								else
								{
									// Player is hiring an army
									if (currentGameBoard.BuyingArmy) 
									{
										currentGameBoard.BuyingArmyCity = currentStructure;
										currentGameBoard.HideAvailableCitiesForArmiesInitial();

                                    }
									// Player is attacking
									else if (currentGameBoard.Attacking)
									{
										// Player is selecting the attack source
										if (currentStructure.PlayerOwner == currentGameBoard.CurrentPlayer)
										{
											currentGameBoard.ShowAvailableCitiesToAttack(currentStructure.Location);
											currentGameBoard.AttackingCity = currentStructure;
										}
										// Player is selecting the attack destination
										else if (currentStructure.PlayerOwner != currentGameBoard.CurrentPlayer && currentStructure.PlayerOwner != -1)
										{
											currentGameBoard.DefendingCity = currentStructure;
											currentGameBoard.ExecuteAttack();
											currentGameBoard.HideAvailableCitiesForAttack ();
											currentGameBoard.HideAvailableCitiesToAttack ();

											if (currentGameBoard.LocalGame.isNetwork)
												NetManager.sendAttackCity(currentGameBoard.AttackingCity.Location.X, currentGameBoard.AttackingCity.Location.Y, currentGameBoard.DefendingCity.Location.X, currentGameBoard.DefendingCity.Location.Y, NetManager.hostConnectionID);

										}
									}
									break;
								}
							}
						}
					}
					if (ourHitObject.GetType() == currentGameBoard.Roads[0].Road_GO.GetType())
					{
						foreach (Road currentRoad in currentGameBoard.Roads)
						{
							if (currentRoad.Road_GO == ourHitObject)
							{
                        if (currentGameBoard.LocalGame.isNetwork)
                        {
                           Debug.Log("Sending Road");
                           NetManager.sendBuildRoad(currentRoad.SideA.X, currentRoad.SideA.Y, currentRoad.SideB.X, currentRoad.SideB.Y, NetManager.hostConnectionID);
                        }

								// Build road
								currentGameBoard.BuildRoad(currentRoad);
								currentGameBoard.HideAvailableRoads();

								break;
							}
						}
					}
					if (ourHitObject.GetType() == currentGameBoard.Tokens[0].GetType()) // This is the clicked token
					{
						for (int z = 0; z < HexTemplate.HEIGHT; z++)
						{
							for (int x = 0; x < HexTemplate.WIDTH; x++)
							{
								if (currentGameBoard.template.hex[x, z].token_go == ourHitObject) // Found the clicked token
								{
									// Move the robber
									currentGameBoard.MoveRobber(currentGameBoard.template.hex[x, z]);
									currentGameBoard.HideHexLocations();

									if (currentGameBoard.LocalGame.isNetwork)
										NetManager.sendMoveRobber(x, z, NetManager.hostConnectionID);
								}
							}
						}
					}
				}
			}
		}
	}
}
