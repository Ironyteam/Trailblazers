using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{

	public GameObject[] hexPrefabs;
	public GameObject hexBoard;
	public GameObject beach;
	public GameObject roadRect;
	public GameObject structure;
	public GameObject city;
	public GameObject token;
	public GameObject armySprite;
	public GameObject armyText;

	public const int WIDTH = HexTemplate.WIDTH;
	public const int HEIGHT = HexTemplate.HEIGHT;

	public HexTemplate template = new HexTemplate();

	float xOffset = 2.9f;
	float zOffset = 3.4f;
	float zPos;


	public List<Road> Roads			  = new List<Road>();
	public List<Structure> Structures = new List<Structure>();
	public List<GameObject> Tokens    = new List<GameObject>();
	public Coordinate hexCoordinate;
	public Coordinate tempCoordinate = new Coordinate(0, 0);
	public Coordinate newCoordinateA;
	public Coordinate newCoordinateB;
	public Structure newStructure;
	public Road newRoad;
	public int structureIndex = 0;
	public int roadIndex = 0;
	public MKGlow MKGlowObject;
	public GuiManager GUIManager;
	public int glowCounter = 0;

	public int CurrentPlayer = 0;
	public bool InitialPlacement = true;
	public bool FirstTurn = true;
	public bool goingUp = true;
	public Game LocalGame;

	public bool BuildSettlementButtonClicked = false;
	public bool BuildRoadButtonClicked = false;
	public bool BuildCityButtonClicked = false;
	public bool BuyingArmy = false;
	public bool Attacking = false;

	public Structure AttackingCity = null;
	public Structure DefendingCity = null;
	public Hex RobberLocation = null;
	/*
  sendEndTurn()
  sendStartTurn()
  sendSendChat(int chatMessageNumber)

    */
	void Start()
	{
		MKGlowObject = GameObject.Find("Main Camera").GetComponent<MKGlow>();
		GUIManager = GameObject.Find("Main Camera").GetComponent<GuiManager>();
		MKGlowObject.BlurSpread = .125f;
		MKGlowObject.BlurIterations = 3;
		MKGlowObject.Samples = 4;


		LocalGame = new Game();

		LocalGame.PlayerList.Add(new Player("Frontiersman"));
		LocalGame.PlayerList.Add(new Player("Engineer"));
		LocalGame.PlayerList.Add(new Player("Knight"));
		LocalGame.PlayerList.Add(new Player("General"));
		LocalGame.PlayerList.Add(new Player("Merchant"));
		LocalGame.PlayerList.Add(new Player("Queen"));


		template = BoardManager.template;

		for (int z = 0; z < HEIGHT; z++)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				// hexPrefab.name = "hex " + x + "," + z;

				zPos = z * zOffset;
				if (x % 2 == 1 || x % 2 == -1)
				{
					zPos += (zOffset * .5f);
				}

				if (template.hex[x, z].resource != -1) 
				{
					// Structure 1
					// if x is odd OR x-1, y != resource	
					if (x % 2 == 1 ||  x == 0 || template.hex [x-1, z].resource == -1)
					{
						// Spawn Structure 1
						spawnStructureOne(x, z);
					}

					// Road 1 - Always spawn
					spawnRoadOne(x, z);

					// Structure 2 - Always spawn
					spawnStructureTwo(x, z);

					// Road 2 - Always spawn
					spawnRoadTwo(x, z);

					// Structure 3 
					// if x is odd OR x+1, y-1 != resource
					if (x % 2 == 1 || z == 0 || template.hex [x+1, z-1].resource == -1) 
					{
						// Spawn Structure 3
						spawnStructureThree(x, z);

						// Spawn Road 3, same conditions apply
						spawnRoadThree(x, z);
					}

					//Structure 4
					if (x % 2 != 1) // x is even
					{   // if x+1, y-1 != resource && x, y-1 != resource
						if (z == 0 || template.hex [x+1, z-1].resource == -1 && template.hex [x, z-1].resource == -1) 
						{
							// Spawn Structure 4
							spawnStructureFour(x, z);
						}
					} 
					else // x is odd
					{	// if x, y-1 != resource
						if (z == 0 || template.hex [x, z-1].resource == -1)
						{
							// Spawn Structure 4
							spawnStructureFour(x, z);
						}
					}

					// Road 4
					// if x, y-1 != resource
					if (z == 0 || template.hex [x, z-1].resource == -1)
					{
						// Spawn Road 4
						spawnRoadFour(x, z);
					}

					// Structure 5
					// if x, y-1 != resource
					if (z == 0 || template.hex [x, z-1].resource == -1)
					{	// x is even
						if (x % 2 != 1)
						{	// First hexagon on board
							if ((x == 0 && z == 0)  || (x != 0 && z == 0) || (x == 0 && z != 0))
							{
								// Spawn Structure 5
								spawnStructureFive(x, z);
							}
							else if (x != 0 && z != 0)
							{
								// if x-1, y-1 != resource
								if (template.hex [x-1, z-1].resource == -1)
								{
									// Spawn Structure 5
									spawnStructureFive(x, z);
								}
							}

						}
						// x is odd
						else 
						{ 
							// if x-1, y != resource
							if (template.hex [x-1, z].resource == -1)
							{
								// Spawn Structure 5
								spawnStructureFive(x, z);
							}
						}
					}

					// Road 5
					// x is even
					if (x % 2 != 1)
					{	// First hexagon on board
						if ((x == 0 && z == 0)  || (x != 0 && z == 0) || (x == 0 && z != 0))
						{
							// Spawn Road 5
							spawnRoadFive(x, z);
						}
						else if (x != 0 && z != 0)
						{
							// if x-1, y-1 != resource
							if (template.hex [x-1, z-1].resource == -1)
							{
								// Spawn Road 5
								spawnRoadFive(x, z);
							}
						}

					}
					// x is odd
					else 
					{ 
						// if x-1, y != resource
						if (template.hex [x-1, z].resource == -1)
						{
							// Spawn Road 5
							spawnRoadFive(x, z);
						}
					}



					//Structure 6
					// x is even
					if (x % 2 != 1)
					{	// First hexagon on board
						if ((x == 0 && z == 0) || (x == 0 && z != 0))
						{
							// Spawn Structure 6
							spawnStructureSix(x, z);
						}
						else if (x != 0 && z != 0)
						{
							// if x-1, y-1 != resource
							if (template.hex [x-1, z-1].resource == -1 && template.hex [x-1, z].resource == -1)
							{
								// Spawn Structure 6
								spawnStructureSix(x, z);
							}
						}

					}
					// x is odd
					else
					{
						// x-1, y != resource
						if (template.hex [x-1, z].resource == -1)
						{
							// Spawn Structure 6
							spawnStructureSix(x, z);
						}
					}

					//Road 6
					// if x is odd OR x-1, y != resource
					if (x % 2 == 1 ||  x == 0 || template.hex [x-1, z].resource == -1)
					{
						// Spawn Road 6
						spawnRoadSix(x, z);
					}

				}

				if (template.hex[x, z].resource >= 0)
				{
					template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefabs[template.hex[x, z].resource], new Vector3(x * xOffset, .2f, zPos), Quaternion.Euler(0, 30, 0));
					Instantiate(beach, new Vector3(x * xOffset, -.2f, zPos), Quaternion.Euler(0, 30, 0));
					template.hex[x, z].hex_go.name = x + "," + z;
					template.hex[x, z].token_go = (GameObject)Instantiate(token, new Vector3(x * xOffset, 2f, zPos), Quaternion.Euler(0, -20, 0));
					Tokens.Add(template.hex[x, z].token_go);

					if (template.hex[x, z].dice_number == 7)
					{
						RobberLocation = template.hex[x, z];
						template.hex[x, z].hasRobber = true;
						template.hex[x, z].token_go.GetComponent<Renderer>().material = GetRobberTokenMaterial();
					}
					else
						template.hex[x, z].token_go.GetComponent<Renderer>().material = GetTokenMaterial(template.hex[x, z].dice_number);

					template.hex[x, z].token_go.GetComponent<Collider>().enabled = false;

					hexCoordinate = new Coordinate(x, z);
					template.hex[x, z].Coordinates[0] = MapUtility.CalculateVerticeOne(hexCoordinate);
					template.hex[x, z].Coordinates[1] = MapUtility.CalculateVerticeTwo(hexCoordinate);
					template.hex[x, z].Coordinates[2] = MapUtility.CalculateVerticeThree(hexCoordinate);
					template.hex[x, z].Coordinates[3] = MapUtility.CalculateVerticeFour(hexCoordinate);
					template.hex[x, z].Coordinates[4] = MapUtility.CalculateVerticeFive(hexCoordinate);
					template.hex[x, z].Coordinates[5] = MapUtility.CalculateVerticeSix(hexCoordinate);

				}
			}
		}

		InitialPlacement = true;
		ShowAvailableSettlementsInitial();
	}

	void FixedUpdate () {

		//if (glowCounter == 100)
		{
			glowCounter = 0;
			if (goingUp)
			{
				if (MKGlowObject.GlowIntensity < .500f)
					MKGlowObject.GlowIntensity += .002f;
				else
				{
					goingUp = false;
					MKGlowObject.GlowIntensity -= .002f;
				}
			}
			else
			{
				if (MKGlowObject.GlowIntensity > .400f)
					MKGlowObject.GlowIntensity -= .002f;
				else
				{
					goingUp = true;
					MKGlowObject.GlowIntensity += .002f;
				}
			}
		}
		glowCounter++;
	}

	void spawnStructureOne(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice One +++ //
		// Calculate coordinates of new structure at vertice one.
		// Rest hexagon coordinates
		hexCoordinate = new Coordinate (xCoord, yCoord);
		newCoordinateA = MapUtility.CalculateVerticeOne(hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - 1f, 0.4f, zPos + 1.6f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset - 1f, 1.6f, zPos + 1.6f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset -.6f, 1.8f, zPos + 1.8f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureTwo(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Two +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice two.
		newCoordinateA = MapUtility.CalculateVerticeTwo (hexCoordinate);
		//Debug.Log (newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + 1f, 0.4f, zPos + 1.6f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset + 1f, 1.6f, zPos + 1.6f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset + 1.4f, 1.8f, zPos + 1.8f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureThree(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Three +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice three.
		newCoordinateA = MapUtility.CalculateVerticeThree(hexCoordinate);
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + 1.9f, 0.4f, zPos - .16f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset + 1.9f, 0.4f, zPos - .16f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset + 2.3f, 0.45f, zPos - .32f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureFour(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Four +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice four.
		newCoordinateA = MapUtility.CalculateVerticeFour(hexCoordinate);
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + 1f, 0.4f, zPos - 1.9f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset + 1f, 1.6f, zPos - 1.9f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 ((xCoord * xOffset) + 1.4f, 1.8f, zPos - 2.1f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureFive(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Five +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice five.
		newCoordinateA = MapUtility.CalculateVerticeFive(hexCoordinate);
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - 1f, 0.4f, zPos - 1.9f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset - 1f, 1.6f, zPos - 1.9f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset -.6f, 1.8f, zPos - 1.7f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureSix(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Six +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice six.
		newCoordinateA = MapUtility.CalculateVerticeSix(hexCoordinate);
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - 1.9f, 0.4f, zPos - .16f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset - 1.9f, 1.6f, zPos - .16f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset - 1.5f, 1.8f, zPos + .04f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnRoadOne(int xCoord, int yCoord)
	{
		// +++ Road One, between Vertice One and Vertice Two +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road one.
		newCoordinateA = MapUtility.CalculateVerticeOne(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road one.
		newCoordinateB = MapUtility.CalculateVerticeTwo(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .020f, .4f, zPos + 1.8f), Quaternion.Euler (0f, 90f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadTwo(int xCoord, int yCoord)
	{
		// +++ Road Two, between Vertice Two and Vertice Three +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road two.
		newCoordinateA = MapUtility.CalculateVerticeTwo(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road two.
		newCoordinateB = MapUtility.CalculateVerticeThree(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + 1.44f, .4f, zPos + .92f), Quaternion.Euler (0f, -30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadThree(int xCoord, int yCoord)
	{
		// +++ Road Three, between Vertice Three and Vertice Four +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road three.
		newCoordinateA = MapUtility.CalculateVerticeThree(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road three.
		newCoordinateB = MapUtility.CalculateVerticeFour(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + 1.48f, .4f, zPos - .84f), Quaternion.Euler (0f, 30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadFour(int xCoord, int yCoord)
	{
		// +++ Road Four, between Vertice Four and Vertice Five +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road four.
		newCoordinateA = MapUtility.CalculateVerticeFour(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road four.
		newCoordinateB = MapUtility.CalculateVerticeFive(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .02f, .4f, zPos - 1.8f), Quaternion.Euler (0f, 90f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadFive(int xCoord, int yCoord)
	{
		// +++ Road Five, between Vertice Five and Vertice Six +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road five.
		newCoordinateA = MapUtility.CalculateVerticeFive(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road five.
		newCoordinateB = MapUtility.CalculateVerticeSix(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset - 1.44f, .4f, zPos - .92f), Quaternion.Euler (0f, -30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadSix(int xCoord, int yCoord)
	{
		// +++ Road Six, between Vertice Six and Vertice One +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road six.
		newCoordinateA = MapUtility.CalculateVerticeSix(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road six.
		newCoordinateB = MapUtility.CalculateVerticeOne(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset - 1.44f, .4f, zPos + .92f), Quaternion.Euler (0f, 30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	// Local building functions
	public void BuildSettlement(Structure settlementTarget)
	{
		MeshRenderer mr = settlementTarget.Structure_GO.GetComponentInChildren<MeshRenderer> ();
		mr.material = GetPlayerMaterial(CurrentPlayer, 1);
		settlementTarget.PlayerOwner = CurrentPlayer;

		// Is not initial placement, charge player resources for building.
		if (!InitialPlacement)
			LocalGame.PlayerList [CurrentPlayer].BuildSettlement();
	}

	public void BuildCity(Structure settlementTarget)
	{
		Vector3 oldStructCoords = settlementTarget.Structure_GO.transform.position;
		MeshRenderer mr;

		Destroy(settlementTarget.Structure_GO);
		settlementTarget.Structure_GO = Instantiate(city, oldStructCoords, Quaternion.identity);
		mr = settlementTarget.Structure_GO.GetComponentInChildren<MeshRenderer>();
		mr.material = GetPlayerMaterial(CurrentPlayer, 2);
		settlementTarget.IsCity = true;

		LocalGame.PlayerList[CurrentPlayer].BuildCity();
	}

	public void BuildRoad(Road targetRoad)
	{
		MeshRenderer mr = targetRoad.Road_GO.GetComponentInChildren<MeshRenderer>();
		targetRoad.PlayerOwner = CurrentPlayer;
		mr.material = GetPlayerMaterial(CurrentPlayer, 0);

		if (!InitialPlacement)
			LocalGame.PlayerList[CurrentPlayer].BuildRoad();

		// Cycles to the next player and shows initial settlements
		if (InitialPlacement)
		{
			if (LocalGame.isNetwork == false)
			{
				NextPlayer();
				// If still initial placement after cycling next player, show settlement locations
				if (InitialPlacement)
					ShowAvailableSettlementsInitial();
			}
			else
				EndTurnNetwork();
		}
		// Longest road logic
		else
		{
			int tempRoadLength = 0;
			tempRoadLength = CalculateLongestRoad(CurrentPlayer);
			if (tempRoadLength > LocalGame.PlayerList[CurrentPlayer].LongestRoad)
			{
				LocalGame.PlayerList[CurrentPlayer].LongestRoad = tempRoadLength;

				if (tempRoadLength > LocalGame.LongestRoad)
				{
					LocalGame.PlayerList[CurrentPlayer].LongestRoadWinner = true;
					if (LocalGame.LongestRoadPlayer != -1)
						LocalGame.PlayerList[LocalGame.LongestRoadPlayer].LongestRoadWinner = true;
					LocalGame.LongestRoadPlayer = CurrentPlayer;
				}

			}
		}
	}

	public void BuyArmy(Structure targetCity)
	{
		targetCity.Armies++;
		LocalGame.PlayerList[CurrentPlayer].HireArmy();
	}

	public void ExecuteAttack()
	{
		if (AttackingCity.Armies > DefendingCity.Armies)
		{
			if (DefendingCity.Armies == 0)
			{
				AttackingCity.Armies--;
			}
			else
			{
				AttackingCity.Armies -= DefendingCity.Armies;
				DefendingCity.Armies = 0;
			}

			Vector3 oldStructCoords = DefendingCity.Structure_GO.transform.position;

			Destroy(DefendingCity.Structure_GO);
			DefendingCity.Structure_GO = Instantiate(structure, oldStructCoords, Quaternion.identity);

			MeshRenderer mr = AttackingCity.Structure_GO.GetComponentInChildren<MeshRenderer>();
			mr = DefendingCity.Structure_GO.GetComponentInChildren<MeshRenderer>();
			mr.material = GetPlayerMaterial(DefendingCity.PlayerOwner, 2);
			DefendingCity.IsCity = false;
			DefendingCity.Structure_GO.GetComponent<Collider>().enabled = false;

		}
		else if (AttackingCity.Armies < DefendingCity.Armies)
		{
			DefendingCity.Armies -= AttackingCity.Armies;
			AttackingCity.Armies = 0;
		}
		else // Armies are equal
		{
			AttackingCity.Armies = 0;
			DefendingCity.Armies = 0;
		}
	}

	public void MoveRobber(Hex hexTarget)
	{
		MeshRenderer mr = hexTarget.token_go.GetComponentInChildren<MeshRenderer>();
		mr.material = GetRobberTokenMaterial();
		hexTarget.hasRobber = true;

		mr = RobberLocation.token_go.GetComponentInChildren<MeshRenderer>();
		mr.material = GetTokenMaterial(RobberLocation.dice_number);
		RobberLocation.hasRobber = false;
		StealResources(hexTarget);
		RobberLocation = hexTarget;
	}


	// Network building functions
	public void BuildSettlementNetwork(int xCoord, int yCoord)
	{
		foreach (Structure targetStructure in Structures)
		{ 
			if (targetStructure.Location.X == xCoord && targetStructure.Location.Y == yCoord)
			{
				BuildSettlement(targetStructure);
				break;    
			}
		}
	}

	public void BuildCityNetwork(int xCoord, int yCoord)
	{
		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoord && targetStructure.Location.Y == yCoord)
			{
				BuildCity(targetStructure);
				break;
			}
		}
	}

	public void BuildRoadNetwork(int xSideA, int ySideA, int xSideB, int ySideB)
	{
		foreach (Road targetRoad in Roads)
		{
			if ((targetRoad.SideA.X == xSideA && targetRoad.SideA.Y == ySideA && targetRoad.SideB.X == xSideB && targetRoad.SideB.Y == ySideB)
				|| (targetRoad.SideA.X == xSideB && targetRoad.SideA.Y == ySideB && targetRoad.SideB.X == xSideA && targetRoad.SideB.Y == ySideA))
			{
				BuildRoad(targetRoad);
				break;
			}
		}
	}

	public void BuyArmyNetwork(int xCoord, int yCoord)
	{
		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoord && targetStructure.Location.Y == yCoord)
			{
				BuyArmy(targetStructure);
				break;
			}
		}
	}

	public void ExecuteAttackNetwork(int xCoordAttacker, int yCoordAttacker, int xCoordDefender, int yCoordDefender)
	{
		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoordAttacker && targetStructure.Location.Y == yCoordAttacker)
			{
				AttackingCity = targetStructure;
				break;
			}
		}

		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoordDefender && targetStructure.Location.Y == yCoordDefender)
			{
				DefendingCity = targetStructure;
				break;
			}
		}

		ExecuteAttack();
	}

	public void MoveRobberNetwork(int xCoord, int yCoord)
	{
		MoveRobber(template.hex[xCoord, yCoord]);
	}

	public void EndTurnNetwork()
	{
		// No code yet
		NextPlayer();
	}




	public void ShowAvailableSettlementsInitial()
	{
		bool isAvailable;
		foreach (Structure currentStructure in Structures)
		{
			isAvailable = true;

			if (currentStructure.PlayerOwner == -1)
			{
				foreach (Road currentRoad in Roads)
				{
					if (CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB))
					{
						foreach (Structure innerStructure in Structures)
						{
							if (CompareCoordinates(innerStructure.Location, currentRoad.SideA) || CompareCoordinates(innerStructure.Location, currentRoad.SideB))
							{
								if (innerStructure.PlayerOwner != -1)
									isAvailable = false;
							}
						}
					}
				}
				if (isAvailable)
				{
					currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
					currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 1);
				}
			}
		}
	}

	public void ShowAvailableSettlements()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuildSettlement())
		{
			bool isAvailable;

			foreach (Road currentRoad in Roads)
			{
				if (currentRoad.PlayerOwner == CurrentPlayer)
				{
					foreach (Structure currentStructure in Structures)
					{
						isAvailable = true;

						if (CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB))
						{
							if (currentStructure.PlayerOwner == -1)
							{
								foreach (Road innerRoad in Roads)
								{
									if (CompareCoordinates(currentStructure.Location, innerRoad.SideA) || CompareCoordinates(currentStructure.Location, innerRoad.SideB))
									{
										foreach (Structure innerStructure in Structures)
										{
											if (CompareCoordinates(innerStructure.Location, innerRoad.SideA) || CompareCoordinates(innerStructure.Location, innerRoad.SideB))
											{
												if (innerStructure.PlayerOwner != -1)
													isAvailable = false;
											}
										}
									}
								}

								if (isAvailable)
								{
									currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
									currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 1);
								}
							}
						}
					}
				}
			}
		}
	}

	public void HideAvailableSettlements()
	{
		BuildSettlementButtonClicked = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == -1)
			{
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(6, 1);
			}
			currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		}
		GUIManager.UpdatePlayer();
	}

	public void ShowAvailableSettlementsToUpgrade()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuildCity())
		{
			foreach (Structure currentStructure in Structures)
			{
				if (currentStructure.PlayerOwner == CurrentPlayer && !currentStructure.IsCity)
				{
					currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
					currentStructure.Structure_GO.GetComponent<Renderer>().material = GetGlowingPlayerMaterial(CurrentPlayer, 1);
				}
			}
		}
	}

	public void HideAvailableSettlementsToUpgrade()
	{
		BuildCityButtonClicked = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == CurrentPlayer)
			{
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(CurrentPlayer, 1);
			}
			currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		}
		GUIManager.UpdatePlayer();
	}

	public void ShowAvailableCitiesForArmies()
	{
		BuyingArmy = true;

		foreach (Structure currentStructure in Structures) 
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity) {
				currentStructure.Structure_GO.GetComponent<Collider> ().enabled = true;
				currentStructure.Structure_GO.GetComponent<Renderer> ().material = GetGlowingPlayerMaterial (CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<TextMesh>().text = currentStructure.Armies.ToString ();
			}

		}
	}

	public void HideAvailableCitiesForArmies()
	{
		BuyingArmy = false;

		foreach (Structure currentStructure in Structures) 
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity) {
				currentStructure.Structure_GO.GetComponent<Collider> ().enabled = false;
				currentStructure.Structure_GO.GetComponent<Renderer> ().material = GetPlayerMaterial (CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
			}

		}
	}

	public void ShowAvailableCitiesForAttack()
	{
		Attacking = true;
		bool canAttack = false;
		bool anyFound = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity && currentStructure.Armies > 0) 
			{
				foreach (Road currentRoad in Roads) 
				{
					if (CompareCoordinates (currentRoad.SideA, currentStructure.Location)) 
					{
						foreach (Road innerRoad in Roads) 
						{
							if (CompareCoordinates(currentRoad.SideB, innerRoad.SideA) || CompareCoordinates(currentRoad.SideB, innerRoad.SideB))
							{
								foreach (Structure innerStructure in Structures)
								{
									if (CompareCoordinates(innerRoad.SideA, innerStructure.Location) || CompareCoordinates(innerRoad.SideB, innerStructure.Location))
									{
										if (innerStructure.IsCity && innerStructure.PlayerOwner != CurrentPlayer)
											canAttack = true;
									}
								}
							}
						}
					} 
					else if (CompareCoordinates (currentRoad.SideB, currentStructure.Location)) 
					{
						foreach (Road innerRoad in Roads) 
						{
							if (CompareCoordinates(currentRoad.SideA, innerRoad.SideA) || CompareCoordinates(currentRoad.SideA, innerRoad.SideB))
							{
								foreach (Structure innerStructure in Structures)
								{
									if (CompareCoordinates(innerRoad.SideA, innerStructure.Location) || CompareCoordinates(innerRoad.SideB, innerStructure.Location))
									{
										if (innerStructure.IsCity && innerStructure.PlayerOwner != CurrentPlayer)
											canAttack = true;
									}
								}
							}
						}
					}
				}
			}

			if (canAttack && currentStructure.PlayerOwner == CurrentPlayer) 
			{
				anyFound = true;
				currentStructure.Structure_GO.GetComponent<Collider> ().enabled = true;
				currentStructure.Structure_GO.GetComponent<Renderer> ().material = GetGlowingPlayerMaterial (CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer> ().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer> ().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<TextMesh> ().text = currentStructure.Armies.ToString ();
			}

			canAttack = false;
		}

		if (!anyFound)
			Attacking = false;
	}

	public void HideAvailableCitiesForAttack()
	{
		Attacking = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity)
			{
				currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
			}

		}
	}


	public void ShowAvailableCitiesToAttack(Coordinate AttackingStructureLocation)
	{
		foreach (Road currentRoad in Roads)
		{
			if (CompareCoordinates(AttackingStructureLocation, currentRoad.SideA))
			{
				foreach (Road innerRoad in Roads)
				{
					if (CompareCoordinates (currentRoad.SideB, innerRoad.SideA) || CompareCoordinates (currentRoad.SideB, innerRoad.SideB)) 
					{
						foreach (Structure currentStructure in Structures)
						{
							if (CompareCoordinates (currentStructure.Location, innerRoad.SideA) || CompareCoordinates (currentStructure.Location, innerRoad.SideB)) 
							{
								if (currentStructure.PlayerOwner != -1 && currentStructure.PlayerOwner != CurrentPlayer && currentStructure.IsCity) 
								{
									currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
									currentStructure.Structure_GO.GetComponent<Renderer>().material = GetGlowingPlayerMaterial(CurrentPlayer, 2);
								}
							}
						}

					}
				}
			}
			else if (CompareCoordinates(AttackingStructureLocation, currentRoad.SideB))
			{
				foreach (Road innerRoad in Roads)
				{
					if (CompareCoordinates (currentRoad.SideA, innerRoad.SideA) || CompareCoordinates (currentRoad.SideA, innerRoad.SideB)) 
					{
						foreach (Structure currentStructure in Structures)
						{
							if (CompareCoordinates (currentStructure.Location, innerRoad.SideA) || CompareCoordinates (currentStructure.Location, innerRoad.SideB)) 
							{
								if (currentStructure.PlayerOwner != -1 && currentStructure.PlayerOwner != CurrentPlayer && currentStructure.IsCity) 
								{
									currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
									currentStructure.Structure_GO.GetComponent<Renderer>().material = GetGlowingPlayerMaterial(CurrentPlayer, 2);
								}
							}
						}

					}
				}
			}
		}
	}

	public void HideAvailableCitiesToAttack()
	{
		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.IsCity && currentStructure.PlayerOwner != CurrentPlayer && currentStructure.PlayerOwner != -1)
			{
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(currentStructure.PlayerOwner, 2);
			}
			currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		}
	}

	public void ShowAvailableRoads()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuildRoad())
		{
			foreach (Structure currentStructure in Structures)
			{
				if (currentStructure.PlayerOwner == CurrentPlayer)
				{
					foreach (Road currentRoad in Roads)
					{
						foreach (Road innerRoad in Roads)
						{
							if (currentRoad.PlayerOwner == CurrentPlayer && innerRoad.PlayerOwner == -1)
							{
								bool isAvailable = false;
								if (CompareCoordinates(currentRoad.SideA, innerRoad.SideA))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideA) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}
								else if (CompareCoordinates(currentRoad.SideA, innerRoad.SideB))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideB) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}
								else if (CompareCoordinates(currentRoad.SideB, innerRoad.SideA))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideA) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}
								else if (CompareCoordinates(currentRoad.SideB, innerRoad.SideB))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideB) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}

								if (isAvailable)
								{
									innerRoad.Road_GO.GetComponent<Collider>().enabled = true;
									innerRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 0);
								}
							}

						}

						if ((CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB)) && currentRoad.PlayerOwner == -1)
						{
							currentRoad.Road_GO.GetComponent<Collider>().enabled = true;
							currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 0);
						}
					}
				}
			}
		}
	}

	public void ShowAvailableRoadsInitial(Coordinate initialSettlement)
	{
		foreach (Road currentRoad in Roads)
		{
			if ((CompareCoordinates(initialSettlement, currentRoad.SideA) || CompareCoordinates(initialSettlement, currentRoad.SideB)) && currentRoad.PlayerOwner == -1)
			{
				currentRoad.Road_GO.GetComponent<Collider>().enabled = true;
				currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 0);
			}
		}
	}

	public void HideAvailableRoads()
	{
		BuildRoadButtonClicked = false;

		foreach (Road currentRoad in Roads)
		{
			if (currentRoad.PlayerOwner == -1)
			{
				currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(6, 0);
			}
			currentRoad.Road_GO.GetComponent<Collider>().enabled = false;
		}
		GUIManager.UpdatePlayer();
	}

	public int CalculateLongestRoad(int playerNumber)
	{
		List<int> CountedRoads = new List<int>();
		int finalRoadLength = 0;
		int tempRoadLength;
		Coordinate firstCoord = new Coordinate (-1, -1);

		foreach (Road currentRoad in Roads)
		{
			if (currentRoad.PlayerOwner == playerNumber)
			{ 
				CountedRoads = new List<int>();
				CountedRoads.Add(currentRoad.RoadID);
				tempRoadLength = 1;
				tempRoadLength += CalculateLongestRoadRecursive(currentRoad, firstCoord, CountedRoads, playerNumber);

				if (tempRoadLength > finalRoadLength)
					finalRoadLength = tempRoadLength;
			}
		}

		return finalRoadLength;
	}

	public int CalculateLongestRoadRecursive(Road roadBuilt, Coordinate previousCoordinate, List<int> LocalCountedRoads, int playerNumber)
	{
		int roadLength = 0;
		int tempLargest = 0;
		int tempReturn = 0;

		foreach (Road currentRoad in Roads) 
		{
			if (!LocalCountedRoads.Contains(currentRoad.RoadID) && currentRoad.PlayerOwner == playerNumber && !CompareCoordinates(currentRoad.SideA, previousCoordinate) && !CompareCoordinates(currentRoad.SideB, previousCoordinate))
			{
				if (CompareCoordinates(currentRoad.SideA, roadBuilt.SideA))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideA) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID);
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideA, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
				else if (CompareCoordinates(currentRoad.SideA, roadBuilt.SideB))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideA) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID);
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideA, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
				else if (CompareCoordinates(currentRoad.SideB, roadBuilt.SideA))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideB) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID); 
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideB, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
				else if (CompareCoordinates(currentRoad.SideB, roadBuilt.SideB))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideB) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID); 
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideB, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
			}
		}

		roadLength += tempLargest;
		return roadLength;
	}

	public void ShowHexLocations()
	{
		for (int z = 0; z < HEIGHT; z++) 
		{
			for (int x = 0; x < WIDTH; x++) 
			{
				if (template.hex [x, z].resource != -1 && !template.hex [x, z].hasRobber) 
				{
					MeshRenderer mr = template.hex [x, z].token_go.GetComponentInChildren<MeshRenderer> ();
					mr.material = GetGlowingTokenMaterial (template.hex [x, z].dice_number);
					template.hex[x, z].token_go.GetComponent<Collider>().enabled = true;
				}
			}
		}
	}

	public void HideHexLocations()
	{
		MeshRenderer mr;

		for (int z = 0; z < HEIGHT; z++) 
		{
			for (int x = 0; x < WIDTH; x++) 
			{
				if (template.hex [x, z].resource != -1) 
				{
					if (!template.hex[x, z].hasRobber)
					{
						mr = template.hex[x, z].token_go.GetComponentInChildren<MeshRenderer>();
						mr.material = GetTokenMaterial(template.hex[x, z].dice_number);
					}
					template.hex[x, z].token_go.GetComponent<Collider>().enabled = false;
				}
			}
		}
	}

	public Material GetPlayerMaterial(int playerNumber, int type)
	{
		Material playerMaterial;

		if (type == 0) // is road
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Roads/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Roads/0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Roads/1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Roads/2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Roads/3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Roads/4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Roads/5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else if (type == 1) // is settlement
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Settlements/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Settlements/0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Settlements/1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Settlements/2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Settlements/3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Settlements/4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Settlements/5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else // is city
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Cities/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Cities/0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Cities/1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Cities/2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Cities/3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Cities/4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Cities/5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}

		return playerMaterial;
	}

	public Material GetGlowingPlayerMaterial(int playerNumber, int type)
	{
		Material playerMaterial;

		if (type == 0) // is road
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Roads/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Roads/Glowing 0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Roads/Glowing 1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Roads/Glowing 2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Roads/Glowing 3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Roads/Glowing 4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Roads/Glowing 5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else if (type == 1) // is settlement
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Settlements/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Settlements/Glowing 0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Settlements/Glowing 1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Settlements/Glowing 2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Settlements/Glowing 3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Settlements/Glowing 4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Settlements/Glowing 5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else // is city
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Cities/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Cities/Glowing 0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Cities/Glowing 1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Cities/Glowing 2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Cities/Glowing 3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Cities/Glowing 4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Cities/Glowing 5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}

		return playerMaterial;
	}

	public Material GetTokenMaterial(int tokenNumber)
	{
		Material tokenMaterial;

		switch (tokenNumber)
		{
		case 2:
			tokenMaterial = Resources.Load("Tokens/Token 2", typeof(Material)) as Material; 
			break;
		case 3:
			tokenMaterial = Resources.Load("Tokens/Token 3", typeof(Material)) as Material; 
			break;
		case 4:
			tokenMaterial = Resources.Load("Tokens/Token 4", typeof(Material)) as Material; 
			break;
		case 5:
			tokenMaterial = Resources.Load("Tokens/Token 5", typeof(Material)) as Material; 
			break;
		case 6:
			tokenMaterial = Resources.Load("Tokens/Token 6", typeof(Material)) as Material; 
			break;
		case 7:
			tokenMaterial = Resources.Load("Tokens/Token 7", typeof(Material)) as Material;
			break;
		case 8:
			tokenMaterial = Resources.Load("Tokens/Token 8", typeof(Material)) as Material; 
			break;
		case 9:
			tokenMaterial = Resources.Load("Tokens/Token 9", typeof(Material)) as Material; 
			break;
		case 10:
			tokenMaterial = Resources.Load("Tokens/Token 10", typeof(Material)) as Material; 
			break;
		case 11:
			tokenMaterial = Resources.Load("Tokens/Token 11", typeof(Material)) as Material; 
			break;
		case 12:
			tokenMaterial = Resources.Load("Tokens/Token 12", typeof(Material)) as Material; 
			break;
		default:
			tokenMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
			break;
		}

		return tokenMaterial;
	}

	public Material GetRobberTokenMaterial()
	{
		return Resources.Load("Tokens/Robber", typeof(Material)) as Material;
	}

	public Material GetGlowingTokenMaterial(int tokenNumber)
	{
		Material tokenMaterial;

		switch (tokenNumber)
		{
		case 2:
			tokenMaterial = Resources.Load("Tokens/Glowing token 2", typeof(Material)) as Material; 
			break;
		case 3:
			tokenMaterial = Resources.Load("Tokens/Glowing token 3", typeof(Material)) as Material; 
			break;
		case 4:
			tokenMaterial = Resources.Load("Tokens/Glowing token 4", typeof(Material)) as Material; 
			break;
		case 5:
			tokenMaterial = Resources.Load("Tokens/Glowing token 5", typeof(Material)) as Material; 
			break;
		case 6:
			tokenMaterial = Resources.Load("Tokens/Glowing token 6", typeof(Material)) as Material; 
			break;
		case 7:
			tokenMaterial = Resources.Load("Tokens/Glowing token 7", typeof(Material)) as Material;
			break;
		case 8:
			tokenMaterial = Resources.Load("Tokens/Glowing token 8", typeof(Material)) as Material; 
			break;
		case 9:
			tokenMaterial = Resources.Load("Tokens/Glowing token 9", typeof(Material)) as Material; 
			break;
		case 10:
			tokenMaterial = Resources.Load("Tokens/Glowing token 10", typeof(Material)) as Material; 
			break;
		case 11:
			tokenMaterial = Resources.Load("Tokens/Glowing token 11", typeof(Material)) as Material; 
			break;
		case 12:
			tokenMaterial = Resources.Load("Tokens/Glowing token 12", typeof(Material)) as Material; 
			break;
		default:
			tokenMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
			break;
		}

		return tokenMaterial;
	}

	public bool CompareCoordinates(Coordinate A, Coordinate B)
	{
		bool tempBool;

		if (A.X == B.X && A.Y == B.Y)
			tempBool = true;
		else
			tempBool = false;

		return tempBool;
	}

	public void NextPlayer()
	{
		HideAvailableSettlementsToUpgrade();
		HideAvailableRoads();
		HideAvailableSettlements ();
		BuildRoadButtonClicked = false;
		BuildCityButtonClicked = false;
		BuildSettlementButtonClicked = false;

		if (InitialPlacement)
		{
			if (FirstTurn)
			{
				if (CurrentPlayer < 5)
					CurrentPlayer++;
				else
				{
					FirstTurn = false;
				}

			}
			else
			{
				if (CurrentPlayer > 0)
					CurrentPlayer--;
				else
				{
					InitialPlacement = false;
				}

			}
		}
		else
		{
			if (CurrentPlayer < 5)
				CurrentPlayer++;
			else
			{
				CurrentPlayer = 0;
				DistributeGold();
			}
		}

		GUIManager.UpdatePlayer();
	}

	public void DistributeResources(int rollNumber)
	{
		for (int z = 0; z < HEIGHT; z++)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				if (template.hex[x, z].dice_number == rollNumber && template.hex[x, z].resource >= 0 && template.hex[x, z].hasRobber == false)
				{
					foreach (Structure currentStructure in Structures)
					{
						if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[0]))
						{
							if (currentStructure.PlayerOwner != -1)
								DistributeResource(template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
							break;
						}
					}

					foreach (Structure currentStructure in Structures)
					{
						if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[1]))
						{
							if (currentStructure.PlayerOwner != -1)
								DistributeResource(template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
							break;
						}
					}

					foreach (Structure currentStructure in Structures)
					{
						if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[2]))
						{
							if (currentStructure.PlayerOwner != -1)
								DistributeResource(template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
							break;
						}
					}

					foreach (Structure currentStructure in Structures)
					{
						if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[3]))
						{
							if (currentStructure.PlayerOwner != -1)
								DistributeResource(template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
							break;
						}
					}

					foreach (Structure currentStructure in Structures)
					{
						if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[4]))
						{
							if (currentStructure.PlayerOwner != -1)
								DistributeResource(template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
							break;
						}
					}

					foreach (Structure currentStructure in Structures)
					{
						if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[5]))
						{
							if (currentStructure.PlayerOwner != -1)
								DistributeResource(template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
							break;
						}
					}
				}
			}
		}
	}

	public void DistributeResource(int resourceNumber, int playerNumber, bool isCity)
	{
		switch (resourceNumber)
		{
		case 0:
			LocalGame.PlayerList[playerNumber].Wood += (isCity ? 2 : 1);
			break;
		case 1:
			LocalGame.PlayerList[playerNumber].Ore += (isCity ? 2 : 1);
			break;
		case 2:
			// Desert, move robber code
			break;
		case 3:
			LocalGame.PlayerList[playerNumber].Wool += (isCity ? 2 : 1);
			break;
		case 4:
			LocalGame.PlayerList[playerNumber].Brick += (isCity ? 2 : 1);
			break;
		case 5:
			LocalGame.PlayerList[playerNumber].Wheat += (isCity ? 2 : 1);
			break;
		default:
			break;
		};
	}

	public void StealResources(Hex robbedHex)
	{
		bool?[] stealFromPlayer = new bool?[6];

		stealFromPlayer[0] = null;
		stealFromPlayer[1] = null;
		stealFromPlayer[2] = null;
		stealFromPlayer[3] = null;
		stealFromPlayer[4] = null;
		stealFromPlayer[5] = null;

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[0]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[1]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[2]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[3]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[4]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[5]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		if (stealFromPlayer[0] == true)
			StealResources(0);
		if (stealFromPlayer[1] == true)
			StealResources(1);
		if (stealFromPlayer[2] == true)
			StealResources(2);
		if (stealFromPlayer[3] == true)
			StealResources(3);
		if (stealFromPlayer[4] == true)
			StealResources(4);
		if (stealFromPlayer[5] == true)
			StealResources(5);

		GUIManager.UpdatePlayer();
	}


	public void DistributeGold()
	{
		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner != -1)
			{
				LocalGame.PlayerList[currentStructure.PlayerOwner].Gold += (currentStructure.IsCity ? 100 : 50);
			}
		}
	}

	public void StealResources(int playerVictim)
	{
		Player victim = LocalGame.PlayerList[playerVictim];
		int resourceCount = victim.Brick + victim.Ore + victim.Wheat + victim.Wood + victim.Wool;
		int resourceToSteal;
		bool resourceStolen;
		resourceCount /= 2;

		for (int x = resourceCount; x > 0; x--)
		{
			resourceStolen = false;
			do
			{
				resourceToSteal = UnityEngine.Random.Range(1, 5);
				switch (resourceToSteal)
				{
				case 1:
					if (victim.Brick > 0)
					{
						victim.Brick--;
						resourceStolen = true;
					}
					break;
				case 2:
					if (victim.Ore > 0)
					{
						victim.Ore--;
						resourceStolen = true;
					}
					break;
				case 3:
					if (victim.Wheat > 0)
					{
						victim.Wheat--;
						resourceStolen = true;
					}
					break;
				case 4:
					if (victim.Wood > 0)
					{
						victim.Wood--;
						resourceStolen = true;
					}
					break;
				case 5:
					if (victim.Wool > 0)
					{
						victim.Wool--;
						resourceStolen = true;
					}
					break;
				default:
					resourceStolen = true;
					Debug.Log("random number was: " + resourceToSteal.ToString());
					break;
				}
			} while (!resourceStolen);
		}
	}

	public void BuyWheat()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyWheat())
		{
			LocalGame.PlayerList[CurrentPlayer].BuyWheat();
			GUIManager.UpdatePlayer();
		}
	}

	public void SellWheat()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Wheat > 0)
		{
			LocalGame.PlayerList[CurrentPlayer].SellWheat();
			GUIManager.UpdatePlayer();
		}
	}

	public void BuyBrick()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyBrick())
		{
			LocalGame.PlayerList[CurrentPlayer].BuyBrick();
			GUIManager.UpdatePlayer();
		}
	}

	public void SellBrick()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Brick > 0)
		{
			LocalGame.PlayerList[CurrentPlayer].SellBrick();
			GUIManager.UpdatePlayer();
		}
	}

	public void BuyWool()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyWool())
		{
			LocalGame.PlayerList[CurrentPlayer].BuyWool();
			GUIManager.UpdatePlayer();
		}
	}

	public void SellWool()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Wool > 0)
		{
			LocalGame.PlayerList[CurrentPlayer].SellWool();
			GUIManager.UpdatePlayer();
		}
	}

	public void BuyWood()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyWood())
		{
			LocalGame.PlayerList[CurrentPlayer].BuyWood();
			GUIManager.UpdatePlayer();
		}
	}

	public void SellWood()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Wood > 0)
		{
			LocalGame.PlayerList[CurrentPlayer].SellWood();
			GUIManager.UpdatePlayer();
		}
	}

	public void BuyOre()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyOre())
		{
			LocalGame.PlayerList[CurrentPlayer].BuyOre();
			GUIManager.UpdatePlayer();
		}
	}

	public void SellOre()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Ore > 0)
		{
			LocalGame.PlayerList[CurrentPlayer].SellOre();
			GUIManager.UpdatePlayer();
		}
	}

	public void BuildSettlementClick()
	{
		HideAvailableSettlementsToUpgrade();
		HideAvailableRoads();

		if (BuildSettlementButtonClicked) 
		{
			BuildSettlementButtonClicked = false;
			HideAvailableSettlements ();
		} 
		else 
		{
			BuildSettlementButtonClicked = true;
			ShowAvailableSettlements();
		}
	}

	public void BuildRoadClick()
	{
		HideAvailableSettlementsToUpgrade();
		HideAvailableSettlements();

		if (BuildRoadButtonClicked) 
		{
			BuildRoadButtonClicked = false;
			HideAvailableRoads();
		} 
		else 
		{
			BuildRoadButtonClicked = true;
			ShowAvailableRoads();
		}
	}

	public void BuildCityClick()
	{
		HideAvailableRoads();
		HideAvailableSettlements();

		if (BuildCityButtonClicked) 
		{
			BuildCityButtonClicked = false;
			HideAvailableSettlementsToUpgrade();
		} 
		else 
		{
			BuildCityButtonClicked = true;
			ShowAvailableSettlementsToUpgrade();
		}
	}

	public void BuyArmyClick()
	{
		if (BuyingArmy) 
		{
			HideAvailableCitiesForArmies();
		} 
		else 
		{
			ShowAvailableCitiesForArmies();
		}
	}

	public void AttackClick()
	{
		if (Attacking) 
		{
			HideAvailableCitiesForAttack();
		} 
		else 
		{
			ShowAvailableCitiesForAttack();
		}
	}
}
